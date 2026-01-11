# Script de sauvegarde SQL pour ComptabiliteDB
# Ce script génère un dump SQL de toutes les données de la base de données

$ErrorActionPreference = "Stop"

$databaseName = "ComptabiliteDB"
$serverName = "(localdb)\mssqllocaldb"
$timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'
$outputFile = "dump_comptabilite_$timestamp.sql"

# Liste des tables à exporter (dans l'ordre pour respecter les contraintes de clés étrangères)
$tables = @(
    "Clients",
    "Postes",
    "Voitures",
    "Factures",
    "LignesFacture",
    "Reservations",
    "Transactions"
)

Write-Host "=============================================" -ForegroundColor Green
Write-Host "Export des données de la base de données" -ForegroundColor Green
Write-Host "Base de données: $databaseName" -ForegroundColor Cyan
Write-Host "Serveur: $serverName" -ForegroundColor Cyan
Write-Host "Fichier de sortie: $outputFile" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Green
Write-Host ""

# Créer le fichier de sortie avec l'en-tête
$header = @"
-- =============================================
-- Dump SQL de la base de données $databaseName
-- Date de création: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
-- Serveur: $serverName
-- =============================================
-- 
-- Ce fichier contient toutes les données de la base de données
-- Pour restaurer, exécutez ce script dans SQL Server Management Studio
-- ou avec sqlcmd
-- =============================================

USE [master];
GO

-- Créer la base de données si elle n'existe pas
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '$databaseName')
BEGIN
    CREATE DATABASE [$databaseName];
END
GO

USE [$databaseName];
GO

-- Désactiver temporairement les contraintes de clés étrangères pour l'insertion
EXEC sp_MSforeachtable @command1='ALTER TABLE ? NOCHECK CONSTRAINT all';
GO

"@

$header | Out-File -FilePath $outputFile -Encoding UTF8

$totalRows = 0

# Fonction pour échapper les chaînes SQL
function Escape-SqlString {
    param([string]$value)
    if ($null -eq $value) { return "NULL" }
    $value = $value -replace "'", "''"
    $value = $value -replace "`r", ""
    $value = $value -replace "`n", ""
    $value = $value -replace "`t", ""
    return "N'$value'"
}

# Exporter les données de chaque table
foreach ($table in $tables) {
    Write-Host "Export de la table: $table..." -ForegroundColor Yellow
    
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
            Write-Host "  -> $rowCount ligne(s) trouvée(s)" -ForegroundColor Cyan
            $totalRows += $rowCount
            
            # Ajouter un commentaire dans le fichier
            "`n-- =============================================" | Out-File -FilePath $outputFile -Append -Encoding UTF8
            "-- Données de la table: $table ($rowCount ligne(s))" | Out-File -FilePath $outputFile -Append -Encoding UTF8
            "-- =============================================`n" | Out-File -FilePath $outputFile -Append -Encoding UTF8
            
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
                Write-Host "  -> Impossible de récupérer les colonnes" -ForegroundColor Red
                continue
            }
            
            $columnsList = ($columns | ForEach-Object { "[$_]" }) -join ", "
            
            # Exporter les données ligne par ligne
            $dataQuery = "USE [$databaseName]; SELECT * FROM [$table];"
            $dataResult = sqlcmd -S $serverName -E -Q $dataQuery -h -1 -W -s "|"
            
            if ($dataResult) {
                $isFirstLine = $true
                $insertStatements = @()
                
                foreach ($line in $dataResult) {
                    if ($isFirstLine) {
                        $isFirstLine = $false
                        continue  # Ignorer la ligne d'en-tête
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
                                $valueList += (Escape-SqlString $value)
                            }
                            elseif ($colInfo.Type -match '^(datetime|datetime2|date|time|smalldatetime)$') {
                                if ($value -match '^\d{4}-\d{2}-\d{2}') {
                                    $valueList += "'$value'"
                                } else {
                                    $valueList += (Escape-SqlString $value)
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
                                $valueList += (Escape-SqlString $value)
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
                    Write-Host "  -> Export réussi: $($insertStatements.Count) ligne(s)" -ForegroundColor Green
                } else {
                    Write-Host "  -> Aucune donnée exportée" -ForegroundColor Yellow
                }
            } else {
                Write-Host "  -> Aucune donnée récupérée" -ForegroundColor Yellow
            }
        } else {
            Write-Host "  -> Table vide ou inexistante, ignorée" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "  -> Erreur lors de l'export: $_" -ForegroundColor Red
        Write-Host "     Détails: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Réactiver les contraintes de clés étrangères
$footer = @"

-- Réactiver les contraintes de clés étrangères
EXEC sp_MSforeachtable @command1='ALTER TABLE ? CHECK CONSTRAINT all';
GO

-- =============================================
-- Fin du dump
-- Total de lignes exportées: $totalRows
-- =============================================
"@

$footer | Out-File -FilePath $outputFile -Append -Encoding UTF8

Write-Host ""
Write-Host "=============================================" -ForegroundColor Green
Write-Host "Export terminé!" -ForegroundColor Green
Write-Host "Fichier de sauvegarde créé: $outputFile" -ForegroundColor Cyan
if (Test-Path $outputFile) {
    $fileInfo = Get-Item $outputFile
    $fileSize = [math]::Round($fileInfo.Length / 1KB, 2)
    Write-Host "Taille du fichier: $fileSize KB" -ForegroundColor Cyan
    Write-Host "Total de lignes exportées: $totalRows" -ForegroundColor Cyan
}
Write-Host "=============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Pour restaurer ces données, exécutez:" -ForegroundColor Yellow
Write-Host "  sqlcmd -S $serverName -E -i `"$outputFile`"" -ForegroundColor White
Write-Host ""
