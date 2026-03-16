# TodoApi - Projet de Conteneurisation

Ce projet est une **API REST développée en ASP.NET Core 8**.
Elle est entièrement **conteneurisée** afin de garantir un **environnement d'exécution reproductible et portable**.

---

## 🏗 Architecture Docker

Le projet utilise **deux approches complémentaires** :

### Dockerfile

Utilisation d'un **Multi-stage build** :

* **SDK** pour compiler l'application
* **Runtime** pour exécuter l'application

Cela permet de créer **une image Docker optimisée et légère**.

### Docker Compose

Utilisation de **Docker Compose** pour l’**orchestration du système**, permettant de :

* Déployer simultanément **l'API**
* Déployer la **base de données MongoDB**
* Les connecter dans un **réseau privé isolé**

---

## 🚀 Guide d'exécution

### 1️⃣ Prérequis

Assurez-vous d'avoir :

* **Docker Desktop** installé
* Docker **en cours d'exécution**

---

### 2️⃣ Lancer l'infrastructure complète

Depuis la **racine du projet** (là où se trouve le fichier `docker-compose.yml`), exécutez :

```bash
docker-compose up -d --build
```

---

### 3️⃣ Accéder à l'application

Une fois les conteneurs démarrés, ouvrez votre navigateur à l'adresse suivante :

**Swagger UI**

```
http://localhost:8080/swagger
```
Et on aura acces au api.
