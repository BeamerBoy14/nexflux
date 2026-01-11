# Script de sauvegarde COMPLÈTE pour ComptabiliteDB
# Ce script génère un dump SQL avec la STRUCTURE (tables) + toutes les DONNÉES
# Parfait pour restaurer sur une base de données distante

$ErrorActionPreference = "Stop"

$databaseName = "ComptabiliteDB"
$serverName = "(localdb)\mssqllocaldb"
$timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'
$outputFile = "dump_complet_$timestamp.sql"

Write-Host "=============================================" -ForegroundColor Green
Write-Host "Génération d'un dump COMPLET (Structure + Données)" -ForegroundColor Green
Write-Host "Base de données: $databaseName" -ForegroundColor Cyan
Write-Host "Serveur: $serverName" -ForegroundColor Cyan
Write-Host "Fichier de sortie: $outputFile" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Green
Write-Host ""

# Étape 1: Générer le schéma complet (structure des tables)
Write-Host "Étape 1/2: Génération du schéma de la base de données..." -ForegroundColor Yellow
$schemaFile = "temp_schema_$timestamp.sql"

try {
    dotnet ef migrations script --output $schemaFile 2>&1 | Out-Null
    
    if (Test-Path $schemaFile) {
        Write-Host "  -> Schéma généré avec succès" -ForegroundColor Green
    } else {
        Write-Host "  -> Erreur: Impossible de générer le schéma" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "  -> Erreur lors de la génération du schéma: $_" -ForegroundColor Red
    exit 1
}

# Étape 2: Générer le dump des données
Write-Host "Étape 2/2: Export des données..." -ForegroundColor Yellow

# Créer l'en-tête du fichier final
$header = @"
-- =============================================
-- DUMP COMPLET de la base de données $databaseName
-- Date de création: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
-- Serveur source: $serverName
-- =============================================
-- 
-- Ce fichier contient:
--   1. La STRUCTURE complète de la base de données (tables, contraintes, index)
--   2. Toutes les DONNÉES existantes
-- 
-- Pour restaurer sur une base de données distante:
--   1. Modifiez le nom de la base de données ci-dessous si nécessaire
--   2. Exécutez ce script dans SQL Server Management Studio ou avec sqlcmd
-- 
-- Exemple avec sqlcmd:
--   sqlcmd -S VOTRE_SERVEUR -U VOTRE_USER -P VOTRE_PASSWORD -i "$outputFile"
-- 
-- =============================================

USE [master];
GO

-- Créer la base de données si elle n'existe pas
-- MODIFIEZ LE NOM ICI si votre base distante a un nom différent
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '$databaseName')
BEGIN
    CREATE DATABASE [$databaseName];
END
GO

USE [$databaseName];
GO

"@

$header | Out-File -FilePath $outputFile -Encoding UTF8

# Ajouter le schéma (structure)
Write-Host "  -> Ajout de la structure des tables..." -ForegroundColor Cyan
Get-Content $schemaFile -Encoding UTF8 | Out-File -FilePath $outputFile -Append -Encoding UTF8

# Ajouter un séparateur
"`n-- =============================================" | Out-File -FilePath $outputFile -Append -Encoding UTF8
"-- DONNÉES - Début de l'import des données" | Out-File -FilePath $outputFile -Append -Encoding UTF8
"-- =============================================`n" | Out-File -FilePath $outputFile -Append -Encoding UTF8

# Désactiver les contraintes avant l'insertion des données
"`n-- Désactiver temporairement les contraintes de clés étrangères pour l'insertion" | Out-File -FilePath $outputFile -Append -Encoding UTF8
"EXEC sp_MSforeachtable @command1='ALTER TABLE ? NOCHECK CONSTRAINT all';" | Out-File -FilePath $outputFile -Append -Encoding UTF8
"GO`n" | Out-File -FilePath $outputFile -Append -Encoding UTF8

# Liste des tables à exporter (dans l'ordre pour respecter les contraintes)
$tables = @(
    "Clients",
    "Postes",
    "Voitures",
    "Factures",
    "LignesFacture",
    "Reservations",
    "Transactions"
)

$totalRows = 0

# Exporter les données de chaque table
foreach ($table in $tables) {
    Write-Host "  -> Export de la table: $table..." -ForegroundColor Gray
    
    try {
        # Vérifier si la table existe et contient des données
        $checkQuery = "USE [$databaseName]; SELECT COUNT(*) FROM [$table];"
        $checkResult = sqlcmd -S $serverName -E -Q $checkQuery -h -1 -W
        
        $rowCount = 0
        if ($checkResult) {
            $countLine = $checkResult | Where-Object { $_ -match '^\s*\d+\s*$' } | Select-Object -First 1
            if ($countLine) {
                $rowCount = [int]($countLine.Trim())
            }
        }
        
        if ($rowCount -gt 0) {
            Write-Host "     $rowCount ligne(s)" -ForegroundColor DarkGray
            $totalRows += $rowCount
            
            # Ajouter un commentaire
            "`n-- Données de la table: $table ($rowCount ligne(s))" | Out-File -FilePath $outputFile -Append -Encoding UTF8
            
            # Obtenir la liste des colonnes
            $columnsQuery = @"
USE [$databaseName];
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = '$table' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;
"@
            
            $columnsResult = sqlcmd -S $serverName -E -Q $columnsQuery -h -1 -W -s "|"
            $columns = @()
            $columnTypes = @{}
            
            if ($columnsResult) {
                foreach ($line in $columnsResult) {
                    if ($line -match '^\s*([^|]+)\|([^|]+)\|([^|]+)\s*$') {
                        $colName = $matches[1].Trim()
                        $dataType = $matches[2].Trim()
                        $isNullable = $matches[3].Trim()
                        $columns += $colName
                        $columnTypes[$colName] = @{
                            Type = $dataType
                            Nullable = $isNullable -eq 'YES'
                        }
                    }
                }
            }
            
            if ($columns.Count -eq 0) {
                continue
            }
            
            $columnsList = ($columns | ForEach-Object { "[$_]" }) -join ", "
            
            # Exporter les données
            $dataQuery = "USE [$databaseName]; SELECT * FROM [$table];"
            $dataResult = sqlcmd -S $serverName -E -Q $dataQuery -h -1 -W -s "|"
            
            if ($dataResult) {
                $isFirstLine = $true
                $insertStatements = @()
                
                foreach ($line in $dataResult) {
                    if ($isFirstLine) {
                        $isFirstLine = $false
                        continue
                    }
                    
                    if ($line.Trim() -eq "" -or $line -match '^[\s|-]+$') {
                        continue
                    }
                    
                    $values = $line -split '\|'
                    $valueList = @()
                    
                    for ($i = 0; $i -lt $columns.Count; $i++) {
                        if ($i -lt $values.Length) {
                            $value = $values[$i].Trim()
                            $colName = $columns[$i]
                            $colInfo = $columnTypes[$colName]
                            
                            if ($value -eq "" -or $value -eq "NULL") {
                                $valueList += "NULL"
                            }
                            elseif ($colInfo.Type -match '^(varchar|nvarchar|char|nchar|text|ntext)$') {
                                $escaped = $value -replace "'", "''"
                                $escaped = $escaped -replace "`r", ""
                                $escaped = $escaped -replace "`n", ""
                                $escaped = $escaped -replace "`t", ""
                                $valueList += "N'$escaped'"
                            }
                            elseif ($colInfo.Type -match '^(datetime|datetime2|date|time|smalldatetime)$') {
                                if ($value -match '^\d{4}-\d{2}-\d{2}') {
                                    $valueList += "'$value'"
                                } else {
                                    $escaped = $value -replace "'", "''"
                                    $valueList += "N'$escaped'"
                                }
                            }
                            elseif ($colInfo.Type -match '^(decimal|numeric|float|real|money|smallmoney)$') {
                                $valueList += $value
                            }
                            elseif ($colInfo.Type -eq 'bit') {
                                $valueList += if ($value -eq '1' -or $value -eq 'True') { '1' } else { '0' }
                            }
                            elseif ($colInfo.Type -match '^(int|bigint|smallint|tinyint)$') {
                                $valueList += $value
                            }
                            else {
                                $escaped = $value -replace "'", "''"
                                $valueList += "N'$escaped'"
                            }
                        } else {
                            $valueList += "NULL"
                        }
                    }
                    
                    $valuesList = $valueList -join ", "
                    $insertStatements += "INSERT INTO [$table] ($columnsList) VALUES ($valuesList);"
                }
                
                if ($insertStatements.Count -gt 0) {
                    $insertStatements | Out-File -FilePath $outputFile -Append -Encoding UTF8
                }
            }
        }
    }
    catch {
        Write-Host "     Erreur: $_" -ForegroundColor Red
    }
}

# Réactiver les contraintes
$footer = @"

-- Réactiver les contraintes de clés étrangères
EXEC sp_MSforeachtable @command1='ALTER TABLE ? CHECK CONSTRAINT all';
GO

-- =============================================
-- Fin du dump complet
-- Total de lignes de données exportées: $totalRows
-- =============================================
"@

$footer | Out-File -FilePath $outputFile -Append -Encoding UTF8

# Nettoyer le fichier temporaire
if (Test-Path $schemaFile) {
    Remove-Item $schemaFile -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "=============================================" -ForegroundColor Green
Write-Host "Export COMPLET terminé!" -ForegroundColor Green
Write-Host "Fichier créé: $outputFile" -ForegroundColor Cyan
if (Test-Path $outputFile) {
    $fileInfo = Get-Item $outputFile
    $fileSize = [math]::Round($fileInfo.Length / 1KB, 2)
    Write-Host "Taille: $fileSize KB" -ForegroundColor Cyan
    Write-Host "Données exportées: $totalRows lignes" -ForegroundColor Cyan
}
Write-Host "=============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Ce fichier contient TOUT ce qu'il faut pour restaurer" -ForegroundColor Yellow
Write-Host "votre base de données sur un serveur distant!" -ForegroundColor Yellow
Write-Host ""
