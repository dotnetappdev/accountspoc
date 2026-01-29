// Simple Chart.js Wrapper for Blazor
window.ChartManager = {
    charts: {},
    
    createChart: function(canvasId, config) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error('Canvas not found:', canvasId);
            return false;
        }
        
        const ctx = canvas.getContext('2d');
        
        // Destroy existing chart if present
        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }
        
        // Create chart using Chart.js
        try {
            this.charts[canvasId] = new Chart(ctx, config);
            return true;
        } catch (error) {
            console.error('Error creating chart:', error);
            return false;
        }
    },
    
    updateChart: function(canvasId, data) {
        if (!this.charts[canvasId]) {
            console.error('Chart not found:', canvasId);
            return false;
        }
        
        this.charts[canvasId].data = data;
        this.charts[canvasId].update();
        return true;
    },
    
    destroyChart: function(canvasId) {
        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
            delete this.charts[canvasId];
            return true;
        }
        return false;
    }
};
