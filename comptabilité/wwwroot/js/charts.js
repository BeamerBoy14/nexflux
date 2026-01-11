let depensesChart = null;

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
    const textColor = '#ffffff';
    const gridColor = 'rgba(255, 255, 255, 0.1)';
    const borderColor = 'rgba(255, 255, 255, 0.2)';

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
                                return (value >= 0 ? '+' : '') + value.toFixed(2) + ' €';
                            }
                        },
                        grid: {
                            color: gridColor,
                            drawBorder: false
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
                            color: textColor,
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
                        backgroundColor: 'rgba(26, 11, 46, 0.95)',
                        backdropFilter: 'blur(10px)',
                        titleColor: textColor,
                        bodyColor: textColor,
                        borderColor: 'rgba(236, 72, 153, 0.5)',
                        borderWidth: 2,
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
                                const sign = value >= 0 ? '+' : '';
                                return label + ': ' + sign + value.toFixed(2) + ' €';
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
            { border: '#ec4899', fill: 'rgba(236, 72, 153, 0.1)' },  // Rose vif
            { border: '#3b82f6', fill: 'rgba(59, 130, 246, 0.1)' },   // Bleu vif
            { border: '#06b6d4', fill: 'rgba(6, 182, 212, 0.1)' },   // Cyan
            { border: '#10b981', fill: 'rgba(16, 185, 129, 0.1)' }, // Vert (pour gains)
            { border: '#f59e0b', fill: 'rgba(245, 158, 11, 0.1)' },  // Orange
            { border: '#8b5cf6', fill: 'rgba(139, 92, 246, 0.1)' }, // Violet
            { border: '#ef4444', fill: 'rgba(239, 68, 68, 0.1)' },  // Rouge
            { border: '#14b8a6', fill: 'rgba(20, 184, 166, 0.1)' }   // Turquoise
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
