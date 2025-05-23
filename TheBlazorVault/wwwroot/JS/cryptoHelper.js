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


let coffreDotNetRef = null;

window.setCoffreDotNetRef = function (dotNetRef) {
    window.coffreDotNetRef = dotNetRef;
}


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

window.getDerivedKey = async function () {
    const jsonKey = JSON.parse(localStorage.getItem("derivedKey"));
    return await crypto.subtle.importKey('jwk', jsonKey, {
        name: "AES-GCM",
        length: 256
    }, true, ["encrypt", "decrypt"]);
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

window.addDataToTable = function (data) {
    const { nom, nomUtilisateur, motDePasseIdentifiant, commentaire } = data;
    const table = document.getElementById('coffreData');
    if (!table) {
        alert('Table not found.');
        return;
    }

    const row = table.insertRow();
    const cellNom = row.insertCell(0);
    const cellNomUtilisateur = row.insertCell(1);
    const cellMotDePasse = row.insertCell(2);
    const cellCommentaire = row.insertCell(3);
    cellNom.textContent = nom;
    cellNomUtilisateur.textContent = nomUtilisateur;
    cellMotDePasse.textContent = "************";
    cellMotDePasse.style.cursor = "pointer";
    cellMotDePasse.setAttribute("id", "motDePasse_" + motDePasseIdentifiant);
    cellMotDePasse.onclick = function () {
        if (window.coffreDotNetRef) {
            window.coffreDotNetRef.invokeMethodAsync('GetPassword', motDePasseIdentifiant);
        } else {
            alert('Référence .NET non initialisée.');
        }
    };
    cellCommentaire.textContent = commentaire;
}

window.decrypt = async function (cipherText, iv, tag) {
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

window.decryptAndDisplay = async function (dataArray) {
    for (const data of dataArray) {
        const decryptedNom = await decrypt(data.nameData.cryptedData, data.nameData.iv, data.nameData.tag);
        const decryptedNomUtilisateur = await decrypt(data.userNameData.cryptedData, data.userNameData.iv, data.userNameData.tag);
        const decryptedCommentaire = await decrypt(data.commentData.cryptedData, data.commentData.iv, data.commentData.tag);

        const entree = {
            nom: decryptedNom,
            nomUtilisateur: decryptedNomUtilisateur,
            motDePasseIdentifiant: data.motDePasseIdentifiant,
            commentaire: decryptedCommentaire
        }

        window.addDataToTable(entree);
    }
}

window.getPassword = async function (cryptedPassword) {
    const decryptedPassword = await decrypt(cryptedPassword.cypher, cryptedPassword.iv, cryptedPassword.tag);

    navigator.clipboard.writeText(decryptedPassword).then(function () {
        alert('Mot de passe copié dans le presse-papiers.');
    }, function (err) {
        console.error('Erreur lors de la copie dans le presse-papiers: ', err);
    });

    passwordElement.style.cursor = "pointer";
}


