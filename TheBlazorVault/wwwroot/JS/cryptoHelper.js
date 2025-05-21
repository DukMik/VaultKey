window.sha256HashString = async function (str) {
    // Encode the string into a Uint8Array
    const encoder = new TextEncoder();
    const data = encoder.encode(str);

    // Use the SubtleCrypto API to hash the data with SHA-256
    const hashBuffer = await crypto.subtle.digest('SHA-256', data);

    // Convert the buffer to a byte array
    return Array.from(new Uint8Array(hashBuffer));
}



