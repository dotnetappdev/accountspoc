// Sidebar Management
window.toggleSidebar = function () {
    const body = document.body;
    body.classList.toggle('sidebar-collapse');
    
    // Save state to localStorage
    const isCollapsed = body.classList.contains('sidebar-collapse');
    localStorage.setItem('sidebarCollapsed', isCollapsed.toString());
};

window.initSidebar = function () {
    // Restore sidebar state from localStorage
    const collapsed = localStorage.getItem('sidebarCollapsed') === 'true';
    if (collapsed) {
        document.body.classList.add('sidebar-collapse');
    } else {
        document.body.classList.remove('sidebar-collapse');
    }
};

// Initialize sidebar state when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', window.initSidebar);
} else {
    window.initSidebar();
}
