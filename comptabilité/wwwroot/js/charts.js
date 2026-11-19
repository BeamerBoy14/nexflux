let depensesChart = null;

const euro = new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', minimumFractionDigits: 2, maximumFractionDigits: 2 });
const euroCourt = new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 });
const signe = (value) => (value > 0 ? '+' : '');

window.initializeDepensesChart = function () {
    console.log('Initialisation du graphique...');
    
    // Vérifier que Chart.js est disponible
    if (typeof Chart === 'undefined') {
        console.error('Chart.js n\'est pas chargé!');
        return;
    }
    
    const ctx = document.getElementById('depensesChart');
    if (!ctx) {
        console.error('Canvas depensesChart non trouvé!');
        return;
    }
    
    console.log('Canvas trouvé, création du graphique...');
    
    // Configuration du thème moderne avec couleurs vives
    const textColor = '#88aaff';
    const gridColor = 'rgba(255, 255, 255, 0.06)';
    const borderColor = 'rgba(0, 204, 255, 0.25)';

    // Une seule instance par canvas : on détruit l'ancienne (retour sur la page)
    if (depensesChart) {
        depensesChart.destroy();
        depensesChart = null;
    }

    try {
        depensesChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [],
                datasets: []
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: {
                    intersect: false,
                    mode: 'index'
                },
                scales: {
                    x: {
                        ticks: {
                            color: textColor,
                            font: {
                                size: 12,
                                weight: '500'
                            }
                        },
                        grid: {
                            color: gridColor,
                            drawBorder: false
                        },
                        border: {
                            color: borderColor
                        }
                    },
                    y: {
                        beginAtZero: false,
                        ticks: {
                            color: textColor,
                            font: {
                                size: 12,
                                weight: '500'
                            },
                            callback: function(value) {
                                return signe(value) + euroCourt.format(value);
                            }
                        },
                        grid: {
                            color: (context) => (context.tick && context.tick.value === 0 ? 'rgba(0, 204, 255, 0.45)' : gridColor),
                            lineWidth: (context) => (context.tick && context.tick.value === 0 ? 2 : 1)
                        },
                        border: {
                            color: borderColor
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                        labels: {
                            color: '#ffffff',
                            font: {
                                size: 12,
                                weight: '600'
                            },
                            padding: 15,
                            usePointStyle: true,
                            pointStyle: 'circle'
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(2, 8, 30, 0.96)',
                        titleColor: '#ffffff',
                        bodyColor: '#ffffff',
                        borderColor: 'rgba(0, 204, 255, 0.5)',
                        borderWidth: 1,
                        padding: 15,
                        titleFont: {
                            size: 14,
                            weight: '700'
                        },
                        bodyFont: {
                            size: 13,
                            weight: '500'
                        },
                        callbacks: {
                            label: function(context) {
                                const label = context.dataset.label || '';
                                const value = context.parsed.y;
                                return label + ' : ' + signe(value) + euro.format(value);
                            }
                        }
                    }
                }
            }
        });
        
        console.log('Graphique créé avec succès');
    } catch (error) {
        console.error('Erreur lors de la création du graphique:', error);
    }
};

window.updateDepensesChart = function (labels, datasets) {
    console.log('Mise à jour du graphique...', { labels: labels.length, datasets: datasets.length });
    
    if (!depensesChart) {
        console.error('Le graphique n\'est pas initialisé!');
        return;
    }
    
    try {
        // Couleurs vives pour les graphiques (style de l'image)
        const colors = [
            { border: '#00ccff', fill: 'rgba(0, 204, 255, 0.10)' },   // Cyan
            { border: '#8b5cf6', fill: 'rgba(139, 92, 246, 0.10)' },  // Violet
            { border: '#f59e0b', fill: 'rgba(245, 158, 11, 0.10)' },  // Ambre
            { border: '#ec4899', fill: 'rgba(236, 72, 153, 0.10)' },  // Rose
            { border: '#10b981', fill: 'rgba(16, 185, 129, 0.10)' },  // Émeraude
            { border: '#3b82f6', fill: 'rgba(59, 130, 246, 0.10)' },  // Bleu
            { border: '#ef4444', fill: 'rgba(239, 68, 68, 0.10)' },   // Rouge
            { border: '#14b8a6', fill: 'rgba(20, 184, 166, 0.10)' }   // Turquoise
        ];
        
        // Convertir les données pour Chart.js avec couleurs vives
        const chartDatasets = datasets.map((dataset, index) => {
            // Pour le solde net, utiliser une couleur cyan/bleu qui change selon le signe
            let finalColor;
            
            if (dataset.label === 'Solde Global Cumulatif' || dataset.label === 'Solde Global') {
                // Couleur distinctive pour le solde global (jaune/orange)
                finalColor = { border: '#FFD700', fill: 'rgba(255, 215, 0, 0.15)' };
            } else if (dataset.label === 'Solde Net' || dataset.label === 'Solde Net Global') {
                // Couleur cyan/bleu pour le solde net
                finalColor = { border: '#00CCFF', fill: 'rgba(0, 204, 255, 0.2)' };
            } else if (dataset.label === 'Gains') {
                // Vert pour les gains
                finalColor = { border: '#10b981', fill: 'rgba(16, 185, 129, 0.2)' };
            } else {
                // Couleur par défaut selon l'index
                const colorIndex = index % colors.length;
                finalColor = colors[colorIndex];
            }
            
            const isSoldeGlobal = dataset.label === 'Solde Global Cumulatif' || dataset.label === 'Solde Global';
            const isCumulatif = dataset.label === 'Solde Global Cumulatif';
            
            return {
                label: dataset.label,
                data: Array.from(dataset.data).map(val => typeof val === 'number' ? val : parseFloat(val)),
                backgroundColor: finalColor.fill,
                borderColor: finalColor.border,
                borderWidth: isSoldeGlobal ? 4 : 3, // Ligne plus épaisse pour le solde global
                borderDash: isCumulatif ? [10, 5] : [], // Ligne en pointillés seulement pour le cumulatif
                fill: !isCumulatif, // Pas de remplissage pour le cumulatif, remplissage pour le solde par période
                tension: isSoldeGlobal ? 0.3 : 0.4,
                pointRadius: isSoldeGlobal ? 5 : 4,
                pointHoverRadius: isSoldeGlobal ? 8 : 6,
                pointBackgroundColor: finalColor.border,
                pointBorderColor: '#ffffff',
                pointBorderWidth: isSoldeGlobal ? 3 : 2,
                pointHoverBackgroundColor: '#ffffff',
                pointHoverBorderColor: finalColor.border,
                pointHoverBorderWidth: isSoldeGlobal ? 4 : 3
            };
        });

        depensesChart.data.labels = labels;
        depensesChart.data.datasets = chartDatasets;
        depensesChart.update('active');
        
        console.log('Graphique mis à jour avec succès');
    } catch (error) {
        console.error('Erreur lors de la mise à jour du graphique:', error);
    }
};
