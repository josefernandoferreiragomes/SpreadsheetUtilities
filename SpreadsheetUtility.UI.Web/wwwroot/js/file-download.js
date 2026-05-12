window.downloadFileFromBytes = (fileName, contentAsBase64) => {
    try {
        // Handle both byte array and base64 string
        let blobData;

        if (typeof contentAsBase64 === 'string') {
            // Already base64 encoded
            const binaryString = atob(contentAsBase64);
            const bytes = new Uint8Array(binaryString.length);
            for (let i = 0; i < binaryString.length; i++) {
                bytes[i] = binaryString.charCodeAt(i);
            }
            blobData = bytes;
        } else if (contentAsBase64 instanceof Uint8Array) {
            // Already bytes
            blobData = contentAsBase64;
        } else if (Array.isArray(contentAsBase64)) {
            // Array of bytes
            blobData = new Uint8Array(contentAsBase64);
        } else {
            throw new Error('Unsupported content type');
        }

        const blob = new Blob([blobData], {
            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        });

        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;
        link.style.display = 'none';

        document.body.appendChild(link);
        link.click();

        // Cleanup
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);

        console.log(`File downloaded successfully: ${fileName}`);
    } catch (error) {
        console.error(`Error downloading file: ${fileName}`, error);
        throw error;
    }
};

window.downloadFileFromUrl =  (url) => {
    try {
        const link = document.createElement('a');
        link.href = url;
        link.style.display = 'none';

        document.body.appendChild(link);
        link.click();

        document.body.removeChild(link);

        console.log(`File download initiated from URL: ${url}`);
    } catch (error) {
        console.error(`Error downloading from URL: ${url}`, error);
        throw error;
    }
};