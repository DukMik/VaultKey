# VaultKey - Secure Password Manager

## Overview
VaultKey is a secure password manager application built with modern technologies. It allows users to store and manage their passwords and sensitive information in secure vaults with strong encryption. This project was developed as part of a French Master's degree program (RNCP level 7).

## Technologies Used
- **Backend**: ASP.NET Core API (.NET 9.0)
- **Frontend**: Blazor Server with Interactive Server rendering
- **UI Framework**: MudBlazor 8.5.1
- **Authentication**: Microsoft Identity (Microsoft.Identity.Web.UI 3.8.3)
- **Database**: SQLite for lightweight, file-based data storage
- **Project Architecture**: Multi-project solution with API, Blazor frontend, and shared DTOs

## Deployment
The application can be launched instantly without complex setup requirements. The SQLite database is automatically created on first run, making deployment simple and straightforward.

## Key Features
- Secure password storage in vaults
- User authentication and authorization
- Interactive UI with MudBlazor components
- API-driven architecture
- Responsive design

## Project Structure
- **TheBlazorVault**: Frontend Blazor application
- **Api**: Backend REST API service
- **TheApiDto**: Shared data transfer objects
- **EntityFrameworkComm**: Database communication layer

## Implementation Details
- Code separation using partial classes for Blazor components
- Clean architecture with services and repositories
- Secure authentication flow
- API communication for data operations

## Development Information
- **Development Timeline**: Approximately 3 weeks
- **Team Size**: 2-3 developers
- **.NET Version**: .NET 9.0

## Future Improvements
Several critical enhancements could be implemented to complete the application:

- **Vault and Entry Management**: 
  - Implement edit functionality for existing vaults and entries
  - Add delete capability for entries to maintain vault cleanliness

- **Enhanced Security**: 
  - Encrypt the decryption key currently stored in local storage
  - Add two-factor authentication options

- **UI/UX Improvements**: 
  - Better leverage MudBlazor component library for a more polished interface
  - Add customizable themes and accessibility features


These improvements were not implemented due to time constraints and the learning curve associated with web development and API technologies, which were new areas for me despite having prior experience with development projects of similar scale.

## Personal Note
This project was developed single-handedly, despite the challenges of learning web development and mastering API implementation. While the current version is not complete, it represents a functional application that I'm proud of at this stage of my learning journey. The code was written with care and attention to best practices as much as possible given my experience level with these specific technologies. I look forward to expanding my knowledge in web development and enhancing this application in the future.

---

# VaultKey - Gestionnaire de Mots de Passe Sécurisé

## Aperçu
VaultKey est une application de gestion de mots de passe sécurisée construite avec des technologies modernes. Elle permet aux utilisateurs de stocker et gérer leurs mots de passe et informations sensibles dans des coffres-forts sécurisés avec un chiffrement robuste. Ce projet a été développé dans le cadre d'un programme de Mastère français (niveau RNCP 7).

## Technologies Utilisées
- **Backend** : API ASP.NET Core (.NET 9.0)
- **Frontend** : Blazor Server avec rendu InteractiveServer
- **Framework UI** : MudBlazor 8.5.1
- **Authentification** : Microsoft Identity (Microsoft.Identity.Web.UI 3.8.3)
- **Base de Données** : SQLite pour un stockage de données léger basé sur fichier
- **Architecture du Projet** : Solution multi-projets avec API, frontend Blazor, et DTOs partagés

## Déploiement
L'application peut être lancée instantanément sans configuration complexe préalable. La base de données SQLite est automatiquement créée lors du premier lancement, rendant le déploiement simple et direct.

## Fonctionnalités Principales
- Stockage sécurisé des mots de passe dans des coffres-forts
- Authentification et autorisation des utilisateurs
- Interface utilisateur interactive avec les composants MudBlazor
- Architecture pilotée par API
- Design responsive

## Structure du Projet
- **TheBlazorVault** : Application frontend Blazor
- **Api** : Service API REST backend
- **TheApiDto** : Objets de transfert de données partagés
- **EntityFrameworkComm** : Couche de communication avec la base de données

## Détails d'Implémentation
- Séparation du code utilisant des classes partielles pour les composants Blazor
- Architecture propre avec services et dépôts
- Flux d'authentification sécurisé
- Communication API pour les opérations de données

## Informations de Développement
- **Durée de Développement** : Environ 3 semaines
- **Taille de l'Équipe** : 2-3 développeurs
- **Version .NET** : .NET 9.0

## Améliorations Futures
Plusieurs améliorations essentielles pourraient être mises en œuvre pour compléter l'application :

- **Gestion des Coffres-forts et des Entrées** : 
  - Implémenter la fonctionnalité de modification des coffres-forts et des entrées existants
  - Ajouter la possibilité de supprimer des entrées pour maintenir les coffres-forts organisés

- **Sécurité Renforcée** : 
  - Chiffrer la clé de déchiffrement actuellement stockée dans le local storage
  - Ajouter des options d'authentification à deux facteurs

- **Améliorations de l'Interface Utilisateur** : 
  - Mieux exploiter la bibliothèque de composants MudBlazor pour une interface plus raffinée
  - Ajouter des thèmes personnalisables et des fonctionnalités d'accessibilité



Ces améliorations n'ont pas été mises en œuvre en raison des contraintes de temps et de la courbe d'apprentissage associée au développement web et aux technologies d'API, qui étaient des domaines nouveaux pour moi malgré une expérience préalable dans des projets de développement d'envergure similaire.

## Note Personnelle
Ce projet a été développé seul, malgré les défis liés à l'apprentissage du développement web et à la maîtrise de l'implémentation des API. Bien que la version actuelle ne soit pas complète, elle représente une application fonctionnelle dont je suis fier à ce stade de mon parcours d'apprentissage. Le code a été écrit avec soin et attention aux meilleures pratiques dans la mesure du possible compte tenu de mon niveau d'expérience avec ces technologies spécifiques. J'attends avec impatience d'élargir mes connaissances en développement web et d'améliorer cette application à l'avenir.
