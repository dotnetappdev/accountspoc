// Helper function to download files from Blazor
window.downloadFile = function (fileName, contentType, byteArray) {
    // Convert byte array to blob
    const blob = new Blob([byteArray], { type: contentType });
    
    // Create a download link
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    
    // Trigger download
    document.body.appendChild(link);
    link.click();
    
    // Cleanup
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
};
