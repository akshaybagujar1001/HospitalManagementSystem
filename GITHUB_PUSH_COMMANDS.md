# Komandat për të push-uar në GitHub

## ✅ Hapat e bërë:
1. ✅ Git repository është inicializuar
2. ✅ Të gjitha fajllat janë shtuar
3. ✅ Commit është bërë

## 📋 Hapat e mbetur:

### 1. Krijo repository në GitHub

1. Shko në [GitHub.com](https://github.com) dhe bëhu login
2. Kliko në butonin **"+"** në këndin e sipërm djathtas → **"New repository"**
3. Jep një emër (p.sh. `hospital-management-system`)
4. Zgjidh **Public** ose **Private**
5. **MOS** krijo README, .gitignore, ose license
6. Kliko **"Create repository"**

### 2. Ekzekuto këto komanda (zëvendëso USERNAME dhe REPO_NAME):

```powershell
# Shko në folderin e projektit
cd "C:\Users\Flori\Desktop\Hospital management system"

# Shto remote repository
git remote add origin https://github.com/USERNAME/REPO_NAME.git

# Ndrysho branch në main
git branch -M main

# Push në GitHub
git push -u origin main
```

### 3. Nëse kërkon authentication:

**Opcioni A: Personal Access Token (PAT)**
1. GitHub.com → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. "Generate new token (classic)"
3. Jep emër dhe zgjidh scope `repo`
4. Kopjo token-in
5. Kur të push-osh, përdor token-in si password (username = GitHub username)

**Opcioni B: GitHub CLI**
```powershell
# Instalo GitHub CLI
winget install GitHub.cli

# Login
gh auth login

# Pastaj push
git push -u origin main
```

### 4. Verifikimi

Shko në repository në GitHub dhe verifiko që të gjitha fajllat janë ngarkuar.

---

## 📝 Për commits të ardhshëm:

```powershell
cd "C:\Users\Flori\Desktop\Hospital management system"
git add .
git commit -m "Përshkrimi i ndryshimeve"
git push
```



