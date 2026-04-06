# Si të ngarkoni projektin në GitHub

## Hapat:

### 1. Krijo një repository në GitHub

1. Shko në [GitHub.com](https://github.com) dhe bëhu login
2. Kliko në butonin **"+"** në këndin e sipërm djathtas dhe zgjidh **"New repository"**
3. Jep një emër për repository (p.sh. `hospital-management-system`)
4. Zgjidh **Public** ose **Private** (sipas preferencës)
5. **MOS** krijo README, .gitignore, ose license (sepse tashmë i kemi)
6. Kliko **"Create repository"**

### 2. Lidh projektin lokal me GitHub

Pas krijimit të repository, GitHub do të shfaqë udhëzime. Përdor këto komanda në terminal:

```bash
# Shko në folderin e projektit
cd "C:\Users\Flori\Desktop\Hospital management system"

# Shto të gjitha fajllat
git add .

# Bëj commit
git commit -m "Initial commit: Hospital Management System with Enterprise Modules"

# Shto remote repository (zëvendëso USERNAME dhe REPO_NAME me të dhënat e tua)
git remote add origin https://github.com/USERNAME/REPO_NAME.git

# Push në GitHub
git branch -M main
git push -u origin main
```

### 3. Nëse ke problem me authentication

Nëse GitHub kërkon authentication, mund të përdorësh:

**Opcioni A: Personal Access Token (PAT)**

1. Shko në GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Kliko "Generate new token (classic)"
3. Jep një emër dhe zgjidh scope `repo`
4. Kopjo token-in
5. Kur të push-osh, përdor token-in si password (username është GitHub username-i yt)

**Opcioni B: GitHub CLI**

```bash
# Instalo GitHub CLI
winget install GitHub.cli

# Login
gh auth login

# Pastaj push normalisht
git push -u origin main
```

### 4. Verifikimi

Pas push, shko në repository në GitHub dhe verifiko që të gjitha fajllat janë ngarkuar.

## Struktura e projektit që do të ngarkohet:

- ✅ Backend (C# .NET Core 8)
- ✅ Frontend (Next.js 14)
- ✅ Documentation
- ✅ React Native app structure
- ❌ node_modules (ignored)
- ❌ bin/obj folders (ignored)
- ❌ .env files (ignored)
- ❌ Database files (ignored)

## Nëse dëshiron të shtosh më shumë commits më vonë:

```bash
git add .
git commit -m "Përshkrimi i ndryshimeve"
git push
```


