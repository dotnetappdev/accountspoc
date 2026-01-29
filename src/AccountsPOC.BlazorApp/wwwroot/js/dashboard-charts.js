// Dashboard Charts Initialization
window.DashboardCharts = {
    initializeCharts: function() {
        // Check if Chart is available (from CDN or local)
        if (typeof Chart === 'undefined') {
            console.warn('Chart.js not loaded, using placeholder charts');
            this.createPlaceholderCharts();
            return;
        }

        this.createSalesChart();
        this.createOrderStatusChart();
        this.createInventoryChart();
    },

    createSalesChart: function() {
        const ctx = document.getElementById('salesChart');
        if (!ctx) return;

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [{
                    label: 'Sales Revenue',
                    data: [45000, 52000, 48000, 61000, 58000, 67000, 72000, 69000, 75000, 82000, 88000, 95000],
                    borderColor: 'rgb(75, 192, 192)',
                    backgroundColor: 'rgba(75, 192, 192, 0.1)',
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top'
                    },
                    title: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return '$' + value.toLocaleString();
                            }
                        }
                    }
                }
            }
        });
    },

    createOrderStatusChart: function() {
        const ctx = document.getElementById('orderStatusChart');
        if (!ctx) return;

        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Completed', 'Pending', 'Processing', 'Cancelled'],
                datasets: [{
                    data: [245, 28, 52, 23],
                    backgroundColor: [
                        'rgba(75, 192, 192, 0.8)',
                        'rgba(255, 206, 86, 0.8)',
                        'rgba(54, 162, 235, 0.8)',
                        'rgba(255, 99, 132, 0.8)'
                    ],
                    borderWidth: 2,
                    borderColor: '#fff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'bottom'
                    }
                }
            }
        });
    },

    createInventoryChart: function() {
        const ctx = document.getElementById('inventoryChart');
        if (!ctx) return;

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Electronics', 'Clothing', 'Food', 'Tools', 'Furniture', 'Sports', 'Books', 'Toys'],
                datasets: [{
                    label: 'In Stock',
                    data: [150, 220, 180, 95, 130, 160, 210, 140],
                    backgroundColor: 'rgba(54, 162, 235, 0.7)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }, {
                    label: 'Low Stock',
                    data: [15, 8, 22, 12, 6, 10, 5, 18],
                    backgroundColor: 'rgba(255, 206, 86, 0.7)',
                    borderColor: 'rgba(255, 206, 86, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Quantity'
                        }
                    },
                    x: {
                        title: {
                            display: true,
                            text: 'Category'
                        }
                    }
                }
            }
        });
    },

    createPlaceholderCharts: function() {
        // Create placeholder SVG charts if Chart.js is not available
        ['salesChart', 'orderStatusChart', 'inventoryChart'].forEach(id => {
            const canvas = document.getElementById(id);
            if (canvas) {
                const parent = canvas.parentElement;
                parent.innerHTML = `
                    <div style="display: flex; align-items: center; justify-content: center; height: 100%; color: #6c757d;">
                        <div style="text-align: center;">
                            <i class="bi bi-graph-up" style="font-size: 3rem; margin-bottom: 1rem;"></i>
                            <p>Chart.js library required</p>
                            <small>Add Chart.js CDN to view interactive charts</small>
                        </div>
                    </div>
                `;
            }
        });
    }
};
