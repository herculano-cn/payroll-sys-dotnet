# Git Repository Setup Guide

This guide will help you initialize a Git repository and push your Payroll Management System to GitHub.

## Prerequisites

- Git installed on your machine
- GitHub account
- GitHub CLI (optional, but recommended)

## Step 1: Initialize Local Repository

```bash
# Navigate to project root
cd /Users/manoel.neto/Projects/aic.payroll-dotnet

# Initialize git repository
git init

# Add all files (respecting .gitignore)
git add .

# Check what will be committed
git status

# Create initial commit
git commit -m "Initial commit: Payroll Management System

- Complete .NET 9 backend API with Clean Architecture
- React + TypeScript frontend with Vite
- Docker configuration for containerization
- SQLite database with EF Core
- Full CRUD operations for employee payroll
- Automatic payroll calculations (INSS, IRRF, FGTS, etc.)
- CNPJ validation
- Health check endpoints
- Comprehensive documentation"
```

## Step 2: Create GitHub Repository

### Option A: Using GitHub CLI (Recommended)

```bash
# Login to GitHub (if not already logged in)
gh auth login

# Create repository
gh repo create payroll-management-system \
  --public \
  --description "Full-stack Payroll Management System with .NET 9 + React + Docker" \
  --source=. \
  --remote=origin \
  --push
```

### Option B: Using GitHub Web Interface

1. Go to https://github.com/new
2. Fill in repository details:
   - **Repository name**: `payroll-management-system`
   - **Description**: `Full-stack Payroll Management System with .NET 9 + React + Docker`
   - **Visibility**: Public or Private
   - **DO NOT** initialize with README, .gitignore, or license (we already have these)
3. Click "Create repository"

## Step 3: Connect Local Repository to GitHub

```bash
# Add remote origin (replace YOUR_USERNAME with your GitHub username)
git remote add origin https://github.com/YOUR_USERNAME/payroll-management-system.git

# Verify remote
git remote -v

# Push to GitHub
git branch -M main
git push -u origin main
```

## Step 4: Verify Upload

```bash
# Check repository status
git status

# View commit history
git log --oneline

# Check remote branches
git branch -a
```

Visit your repository on GitHub:
```
https://github.com/YOUR_USERNAME/payroll-management-system
```

## What Gets Committed

### ✅ Included Files

**Backend (.NET 9)**:
- Source code (`.cs` files)
- Project files (`.csproj`)
- Solution file (`.sln`)
- Dockerfile
- Configuration files

**Frontend (React)**:
- Source code (`.tsx`, `.ts`, `.css`)
- Package configuration (`package.json`, `package-lock.json`)
- Vite configuration
- Dockerfile
- Nginx configuration

**Documentation**:
- README.md
- DOCKER.md
- MODERNIZATION_PLAN.md
- SE_TRACEABILITY_MATRIX.md
- TRANSLATION_DECISIONS.md
- GIT_SETUP.md

**Configuration**:
- docker-compose.yml
- .dockerignore files
- .env.example
- .gitignore

### ❌ Excluded Files (via .gitignore)

**Build Artifacts**:
- `bin/`, `obj/` folders
- `dist/`, `build/` folders
- `node_modules/`

**IDE Files**:
- `.vs/`, `.vscode/`, `.idea/`
- `*.suo`, `*.user`

**Database Files**:
- `*.db`, `*.db-shm`, `*.db-wal`

**Environment Files**:
- `.env` (only `.env.example` is committed)

**OS Files**:
- `.DS_Store`, `Thumbs.db`

**Logs and Temporary Files**:
- `*.log`, `*.tmp`, `*.swp`

## Repository Structure

```
payroll-management-system/
├── backend/                    # .NET 9 API
│   ├── Payroll.API/           # Web API project
│   ├── Payroll.Application/   # Business logic
│   ├── Payroll.Domain/        # Domain entities
│   ├── Payroll.Infrastructure/# Data access
│   ├── Payroll.Tests/         # Unit tests
│   ├── Dockerfile             # Backend container
│   └── Payroll.sln            # Solution file
├── frontend/                   # React + TypeScript
│   ├── src/                   # Source code
│   ├── public/                # Static assets
│   ├── Dockerfile             # Frontend container
│   ├── nginx.conf             # Nginx configuration
│   └── package.json           # Dependencies
├── docker-compose.yml         # Docker orchestration
├── .gitignore                 # Git ignore rules
├── .env.example               # Environment template
├── README.md                  # Main documentation
├── DOCKER.md                  # Docker guide
└── GIT_SETUP.md              # This file
```

## Common Git Commands

### Daily Workflow

```bash
# Check status
git status

# Add changes
git add .

# Commit changes
git commit -m "Description of changes"

# Push to GitHub
git push

# Pull latest changes
git pull
```

### Branching

```bash
# Create new branch
git checkout -b feature/new-feature

# Switch branches
git checkout main

# List branches
git branch -a

# Delete branch
git branch -d feature/old-feature
```

### Viewing History

```bash
# View commit history
git log

# View compact history
git log --oneline --graph

# View changes
git diff

# View file history
git log --follow filename
```

## Best Practices

### Commit Messages

Use clear, descriptive commit messages:

```bash
# Good examples
git commit -m "Add employee validation for CNPJ"
git commit -m "Fix: Resolve Entity Framework tracking conflict"
git commit -m "Docs: Update Docker deployment guide"
git commit -m "Refactor: Improve payroll calculation service"

# Bad examples
git commit -m "fix"
git commit -m "updates"
git commit -m "changes"
```

### Commit Frequency

- Commit often with logical changes
- One feature/fix per commit
- Don't commit broken code to main branch

### Branch Strategy

**For Solo Development**:
```bash
main (production-ready code)
└── feature branches (for new features)
```

**For Team Development**:
```bash
main (production)
├── develop (integration)
└── feature/* (features)
└── hotfix/* (urgent fixes)
```

## GitHub Repository Settings

### Recommended Settings

1. **Branch Protection** (for main branch):
   - Require pull request reviews
   - Require status checks to pass
   - Require branches to be up to date

2. **Security**:
   - Enable Dependabot alerts
   - Enable secret scanning
   - Enable code scanning

3. **Actions** (CI/CD):
   - Set up GitHub Actions for automated builds
   - Configure Docker image builds
   - Set up automated tests

## Adding Collaborators

```bash
# Using GitHub CLI
gh repo add-collaborator YOUR_USERNAME/payroll-management-system COLLABORATOR_USERNAME

# Or via web interface:
# Settings → Collaborators → Add people
```

## Cloning Repository

For others to clone your repository:

```bash
# HTTPS
git clone https://github.com/YOUR_USERNAME/payroll-management-system.git

# SSH (if configured)
git clone git@github.com:YOUR_USERNAME/payroll-management-system.git

# Navigate to project
cd payroll-management-system

# Install dependencies and run
# See README.md for instructions
```

## Troubleshooting

### Large Files

If you accidentally committed large files:

```bash
# Remove from Git but keep locally
git rm --cached large-file.db

# Add to .gitignore
echo "large-file.db" >> .gitignore

# Commit changes
git add .gitignore
git commit -m "Remove large file from Git"
```

### Undo Last Commit

```bash
# Keep changes in working directory
git reset --soft HEAD~1

# Discard changes (careful!)
git reset --hard HEAD~1
```

### Force Push (Use with Caution)

```bash
# Only if you're sure and working alone
git push --force origin main
```

### Authentication Issues

```bash
# Use GitHub CLI for easier authentication
gh auth login

# Or configure SSH keys
# See: https://docs.github.com/en/authentication/connecting-to-github-with-ssh
```

## Next Steps

After pushing to GitHub:

1. **Add README badges**:
   - Build status
   - License
   - Version

2. **Set up GitHub Actions**:
   - Automated builds
   - Docker image publishing
   - Automated tests

3. **Create releases**:
   - Tag versions
   - Create release notes
   - Attach binaries

4. **Add documentation**:
   - Wiki pages
   - API documentation
   - Contributing guidelines

## Resources

- [Git Documentation](https://git-scm.com/doc)
- [GitHub Docs](https://docs.github.com)
- [GitHub CLI](https://cli.github.com/)
- [Git Cheat Sheet](https://education.github.com/git-cheat-sheet-education.pdf)

## Support

If you encounter issues:
1. Check `.gitignore` is working: `git status`
2. Verify remote: `git remote -v`
3. Check GitHub repository settings
4. Review commit history: `git log`