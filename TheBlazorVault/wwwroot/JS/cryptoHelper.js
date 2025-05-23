window.generateSalt = function () {
    return window.crypto.getRandomValues(new Uint8Array(16));
}

window.hashPassword = async function (password) {
    const enc = new TextEncoder();
    const passwordData = enc.encode(password);

    const hashBuffer = await window.crypto.subtle.digest(
        'SHA-256',
        passwordData
    );

    return new Uint8Array(hashBuffer);
};

window.getAndHashPassword = async function () {
    const passwordElement = document.getElementById('coffrePassword');
    if (!passwordElement || !passwordElement.value) {
        alert('Le mot de passe est obligatoire.');
        return;
    }

    const password = passwordElement.value;

    try {
        return await window.hashPassword(password);
    } catch (error) {
        console.error('Error hashing password:', error);
        return null;
    }
};

window.deriveKey = async function (salt) {

    const passwordElement = document.getElementById('coffrePassword');
    if (!passwordElement || !passwordElement.value) {
        alert('Le mot de passe est obligatoire.');
        return;
    }

    const password = passwordElement.value;

    const encoder = new TextEncoder();
    const passwordKey = await window.crypto.subtle.importKey(
        "raw",
        encoder.encode(password),
        { name: "PBKDF2" },
        false,
        ["deriveBits", "deriveKey"]
    );


    const key = await window.crypto.subtle.deriveKey(
        {
            name: "PBKDF2",
            salt: salt,
            iterations: 5000,
            hash: "SHA-256"
        },
        passwordKey,
        { name: "AES-GCM", length: 256 },
        true,
        ["encrypt", "decrypt"]
    );

    const exportedKey = await crypto.subtle.exportKey('jwk', key);
    localStorage.setItem("derivedKey", JSON.stringify(exportedKey));
}


let dotNetReference = null;

window.initFormSubmission = function (dotNetRef) {
    dotNetReference = dotNetRef;
};





window.getInputValue = async function (inputId) {
    const element = document.getElementById(inputId);
    if (!element) {
        alert('Element not found.');
        return;
    }

    if (element.type === "hidden") {
        return new Date().toISOString();
    }

    return element.value;
}

window.encrypt = async function (inputId) {
    let data = await getInputValue(inputId);
    const encoder = new TextEncoder();
    data = encoder.encode(data);

    const key = await getDerivedKey();
    const iv = window.crypto.getRandomValues(new Uint8Array(12));

    const cipherDataWithAuthTag = await window.crypto.subtle.encrypt(
        {
            name: "AES-GCM",
            iv: iv
        },
        key,
        data
    );

    const cipherData = cipherDataWithAuthTag.slice(0, cipherDataWithAuthTag.byteLength - 16);
    const authTag = cipherDataWithAuthTag.slice(cipherDataWithAuthTag.byteLength - 16);

    return {
        cipherData: new Uint8Array(cipherData),
        authTag: new Uint8Array(authTag),
        iv: iv
    };
}

/*
window.decrypt = async function (cipherData, iv, tag) {
    const cipherTextArray = new Uint8Array(Object.values(cipherText));
    const ivArray = new Uint8Array(Object.values(iv));
    const authTagArray = new Uint8Array(Object.values(tag));
    const key = await getDerivedKey();

    const combinedCipherText = new Uint8Array([...cipherTextArray, ...authTagArray]);
    const decoder = new TextDecoder();

    const decryptedData = await window.crypto.subtle.decrypt(
        {
            name: "AES-GCM",
            iv: ivArray,
        },
        key,
        combinedCipherText
    );

    return decoder.decode(decryptedData);
}
*/


