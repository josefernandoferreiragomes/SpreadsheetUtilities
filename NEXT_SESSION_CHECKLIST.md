# 🚀 Next Session Checklist

## Session Goal: Phase 1A - Data Access & Database Foundation

This checklist guides implementation of the Data Access Layer for SpreadsheetUtilities.

**Status**: Ready to start  
**Estimated Time**: 4-6 hours  
**Prerequisites**: All documentation reviewed ✅

---

## Pre-Implementation Review

Before starting, review these sections:

- [ ] Read [IMPLEMENTATION_ROADMAP.md](IMPLEMENTATION_ROADMAP.md) → "Phase 1A: Set Up Data Persistence Foundation"
- [ ] Review [ARCHITECTURE.md](ARCHITECTURE.md) → "Layered Architecture"
- [ ] Check [ARCHITECTURE.md](ARCHITECTURE.md) → "Design Patterns" → "Repository Pattern" and "Unit of Work Pattern"
- [ ] Review [.copilot-instructions](.copilot-instructions) → "New Features: Architecture Guidelines"
- [ ] Understand [CONTRIBUTING.md](CONTRIBUTING.md) → "Making Changes"

**Time**: ~30 minutes

---

## Phase 1A: Implementation Steps

### Step 1: Create Data Access Project ⏳

**What to do**:
- [ ] Create new class library project: `SpreadsheetUtility.Library.DataAccess` (.NET 8)
- [ ] Add NuGet packages:
  - [ ] Microsoft.EntityFrameworkCore 8.0.0
  - [ ] Microsoft.EntityFrameworkCore.SqlServer 8.0.0
  - [ ] Microsoft.EntityFrameworkCore.Tools 8.0.0
  - [ ] Microsoft.EntityFrameworkCore.Design 8.0.0
  - [ ] Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.0
- [ ] Create folder structure:
  - [ ] `DbContexts/`
  - [ ] `Repositories/`
  - [ ] `UnitOfWork/`
  - [ ] `Configurations/`
  - [ ] `Models/`
- [ ] Add to solution file

**Reference**: IMPLEMENTATION_ROADMAP.md - Step 1

**Time**: ~30 minutes

---

### Step 2: Create Identity Project ⏳

**What to do**:
- [ ] Create new class library project: `SpreadsheetUtility.Library.Identity` (.NET 8)
- [ ] Add NuGet packages:
  - [ ] Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.0
  - [ ] System.IdentityModel.Tokens.Jwt 7.0.0
- [ ] Create folder structure:
  - [ ] `Models/`
  - [ ] `Services/`
  - [ ] `DTOs/`
- [ ] Add to solution file

**Reference**: IMPLEMENTATION_ROADMAP.md - Step 2

**Time**: ~20 minutes

---

### Step 3: Create Domain Models ⏳

**What to do**:

#### ApplicationUser.cs
- [ ] Inherit from `IdentityUser<int>`
- [ ] Add properties:
  - [ ] FirstName (string?)
  - [ ] LastName (string?)
  - [ ] CreatedAt (DateTime)
  - [ ] LastLoginAt (DateTime?)
  - [ ] IsActive (bool)
- [ ] Add navigation properties:
  - [ ] Sessions (ICollection<UserSession>)
  - [ ] Projects (ICollection<Project>)

#### UserSession.cs
- [ ] Properties:
  - [ ] Id, UserId, SessionKey
  - [ ] Name, CreatedAt, UpdatedAt, LastAccessedAt
  - [ ] ProjectsJson, TasksJson, DevelopersJson (JSON storage)
  - [ ] IsDeleted flag
- [ ] Navigation: User reference

**Reference**: IMPLEMENTATION_ROADMAP.md - Step 4

**Time**: ~20 minutes

---

### Step 4: Create DbContext ⏳

**What to do**:
- [ ] File: `DbContexts/ApplicationDbContext.cs`
- [ ] Inherit from `IdentityDbContext<ApplicationUser, IdentityRole<int>, int>`
- [ ] Add DbSets:
  - [ ] UserSessions
  - [ ] Projects
  - [ ] SavedTasks
  - [ ] SavedDevelopers
- [ ] Configure in OnModelCreating:
  - [ ] Call base.OnModelCreating
  - [ ] Apply configurations from assembly
- [ ] Add constructor with DbContextOptions

**Reference**: IMPLEMENTATION_ROADMAP.md - Step 3

**Time**: ~20 minutes

---

### Step 5: Implement Repository Pattern ⏳

**What to do**:

#### IGenericRepository.cs
- [ ] Create interface with methods:
  - [ ] GetByIdAsync(int id)
  - [ ] GetAllAsync()
  - [ ] AddAsync(TEntity)
  - [ ] UpdateAsync(TEntity)
  - [ ] DeleteAsync(int id)
  - [ ] CountAsync()
  - [ ] QueryableAsync()

#### GenericRepository.cs
- [ ] Implement IGenericRepository<TEntity>
- [ ] Inject ApplicationDbContext
- [ ] Implement all methods with EF Core calls
- [ ] Add SaveChangesAsync() after Add/Update/Delete

#### IUserRepository.cs
- [ ] Extend IGenericRepository<ApplicationUser>
- [ ] Add methods:
  - [ ] GetByEmailAsync(string)
  - [ ] GetByUsernameAsync(string)
  - [ ] GetActiveUsersAsync()

#### UserRepository.cs
- [ ] Implement IUserRepository
- [ ] Implement custom methods with proper EF queries

**Reference**: IMPLEMENTATION_ROADMAP.md - Step 5

**Time**: ~45 minutes

---

### Step 6: Implement Unit of Work Pattern ⏳

**What to do**:

#### IUnitOfWork.cs
- [ ] Create interface with properties:
  - [ ] IUserRepository Users { get; }
  - [ ] IGenericRepository<UserSession> UserSessions { get; }
  - [ ] IGenericRepository<Project> Projects { get; }
- [ ] Add methods:
  - [ ] BeginTransactionAsync()
  - [ ] CommitAsync()
  - [ ] RollbackAsync()

#### UnitOfWork.cs
- [ ] Implement IUnitOfWork
- [ ] Initialize repositories in constructor
- [ ] Implement transaction methods using IDbContextTransaction

**Reference**: IMPLEMENTATION_ROADMAP.md - Step 6

**Time**: ~30 minutes

---

### Step 7: Create Authentication Service (Identity) ⏳

**What to do**:

#### IAuthenticationService.cs
- [ ] Methods:
  - [ ] RegisterAsync(RegisterRequest)
  - [ ] LoginAsync(LoginRequest)
  - [ ] LogoutAsync(int userId)
  - [ ] ValidateCredentialsAsync(string, string)
  - [ ] IsEmailUniqueAsync(string)

#### DTOs
- [ ] LoginRequest.cs - Email, Password
- [ ] RegisterRequest.cs - Email, Password, FirstName, LastName
- [ ] AuthenticationResponse.cs - User, Token
- [ ] UserDto.cs - Id, Email, FirstName, LastName

#### AuthenticationService.cs
- [ ] Implement IAuthenticationService
- [ ] Use UserManager<ApplicationUser>
- [ ] Implement password hashing validation
- [ ] Add logging

**Reference**: IMPLEMENTATION_ROADMAP.md - Step 7

**Time**: ~45 minutes

---

### Step 8: Update Program.cs ⏳

**What to do**:
- [ ] Add database context registration
- [ ] Add Identity configuration
- [ ] Register repository services
- [ ] Register Unit of Work
- [ ] Add session support
- [ ] Register authentication services
- [ ] Update middleware pipeline (use authentication)

**Reference**: IMPLEMENTATION_ROADMAP.md - "Phase 1B" → Step 1

**Time**: ~20 minutes

---

### Step 9: Create Initial Migration ⏳

**What to do**:
- [ ] Run: `dotnet ef migrations add InitialCreate -p SpreadsheetUtility\SpreadsheetUtility.Library.DataAccess`
- [ ] Review generated migration file
- [ ] Verify Up/Down methods

**Reference**: IMPLEMENTATION_ROADMAP.md - "Phase 1C"

**Time**: ~10 minutes

---

### Step 10: Update Configuration ⏳

**What to do**:
- [ ] Update `appsettings.json`:
  - [ ] Add ConnectionStrings.DefaultConnection
  - [ ] Use: `Server=(localdb)\\mssqllocaldb;Database=SpreadsheetUtilitiesDb;Trusted_Connection=true;`
- [ ] Create `appsettings.Development.json` (git ignored)
- [ ] Update `appsettings.Production.json` (for Azure)

**Reference**: IMPLEMENTATION_ROADMAP.md - "Phase 1C" → Step 2

**Time**: ~10 minutes

---

### Step 11: Apply Migration ⏳

**What to do**:
- [ ] Run: `dotnet ef database update -p SpreadsheetUtility\SpreadsheetUtility.Library.DataAccess`
- [ ] Verify database created in `(localdb)\\mssqllocaldb`
- [ ] Check tables created with Identity schema

**Reference**: IMPLEMENTATION_ROADMAP.md - "Phase 1C" → Step 3

**Time**: ~5 minutes

---

### Step 12: Write Unit Tests ⏳

**What to do**:

#### AuthenticationServiceTests.cs
- [ ] Test RegisterAsync - success
- [ ] Test RegisterAsync - duplicate email
- [ ] Test LoginAsync - success
- [ ] Test LoginAsync - invalid password
- [ ] Test IsEmailUniqueAsync

#### UserRepositoryTests.cs
- [ ] Test GetByEmailAsync
- [ ] Test GetActiveUsersAsync
- [ ] Test base CRUD operations

#### UnitOfWorkTests.cs
- [ ] Test repository access
- [ ] Test transaction flow (if testing DI setup)

**Reference**: CONTRIBUTING.md - Testing section

**Time**: ~1 hour

---

### Step 13: Verify & Document ⏳

**What to do**:
- [ ] Run: `dotnet build` - No errors/warnings
- [ ] Run: `dotnet test` - All tests pass
- [ ] Update CHANGELOG.md:
  - [ ] Add "Added" section for Phase 1A
  - [ ] List all new projects and components
- [ ] Add code comments (public APIs with XML docs)
- [ ] Verify git is clean (no uncommitted changes except config)

**Time**: ~30 minutes

---

## Testing Checklist

### Build & Compilation
- [ ] `dotnet build` runs without errors
- [ ] `dotnet build -c Release` successful
- [ ] No compiler warnings

### Database
- [ ] Database file created at: `(localdb)\mssqllocaldb`
- [ ] All tables created correctly
- [ ] Can run: `dotnet ef database update --verbose`

### Unit Tests
- [ ] All new tests pass: `dotnet test`
- [ ] Tests cover new code (80%+ goal)
- [ ] No test warnings

### Integration
- [ ] Web app still builds after adding projects
- [ ] Can reference new services from Program.cs
- [ ] No circular dependencies

---

## Code Quality Checklist

- [ ] Code follows naming conventions (see .copilot-instructions)
- [ ] Constructor injection used throughout
- [ ] No static methods except in Providers
- [ ] Public APIs have XML documentation
- [ ] SOLID principles followed
- [ ] Repository pattern correctly implemented
- [ ] Unit of Work pattern correctly implemented
- [ ] No hardcoded values (magic numbers/strings)
- [ ] Proper exception handling with meaningful messages
- [ ] Logging added to key methods

---

## Documentation Updates

- [ ] CHANGELOG.md updated with new features
- [ ] Add code comments explaining complex logic
- [ ] Update ARCHITECTURE.md if design changed
- [ ] Add notes about database schema if new entities added
- [ ] Document any new patterns used

---

## Git & Commit Checklist

**Before committing**:
- [ ] No local .env or secrets committed
- [ ] No .vs/ or bin/ or obj/ folders
- [ ] Only .md, .cs, and project files committed

**Commit messages** (follow conventions):
- [ ] `feat: add data access layer with repository pattern`
- [ ] `feat: implement unit of work pattern for transactions`
- [ ] `feat: create authentication service`
- [ ] `feat: add initial EF Core migration`
- [ ] `test: add unit tests for repositories`
- [ ] `docs: update CHANGELOG for Phase 1A`

---

## Troubleshooting During Implementation

### Database Issues

**Problem**: "Cannot connect to (localdb)\mssqllocaldb"
```bash
# Solution: Start LocalDB
sqllocaldb start mssqllocaldb
```

**Problem**: "Migrations pending"
```bash
# Solution: Apply migration
dotnet ef database update
```

**Problem**: "Migration already exists"
```bash
# Solution: Remove and recreate
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
```

### Build Issues

**Problem**: "Cannot find type ApplicationUser"
```bash
# Solution: Rebuild solution
dotnet clean && dotnet build
```

**Problem**: "NuGet package not found"
```bash
# Solution: Restore packages
dotnet restore
```

### Reference Issues

**Problem**: "Cannot resolve IUnitOfWork"
```bash
# Solution: Add to Program.cs
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

---

## Success Criteria

By end of this session, you will have:

✅ **Projects Created**
- [ ] SpreadsheetUtility.Library.DataAccess (.NET 8)
- [ ] SpreadsheetUtility.Library.Identity (.NET 8)

✅ **Patterns Implemented**
- [ ] Repository pattern with generic repository
- [ ] Specialized repositories (UserRepository)
- [ ] Unit of Work pattern
- [ ] Dependency injection integration

✅ **Database Setup**
- [ ] DbContext with all required entities
- [ ] Entity configurations
- [ ] Initial migration created
- [ ] Database created locally

✅ **Services Implemented**
- [ ] Authentication service
- [ ] User management basics
- [ ] DTOs for API contracts

✅ **Quality Assurance**
- [ ] 80%+ unit test coverage
- [ ] All tests passing
- [ ] Code review ready

✅ **Documentation**
- [ ] CHANGELOG.md updated
- [ ] Code documented
- [ ] Committed to git

---

## Files to Create Summary

```
SpreadsheetUtility.Library.DataAccess/
├── DbContexts/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── ApplicationUser.cs
│   ├── UserSession.cs
│   └── [others as needed]
├── Repositories/
│   ├── IGenericRepository.cs
│   ├── GenericRepository.cs
│   ├── IUserRepository.cs
│   └── UserRepository.cs
├── UnitOfWork/
│   ├── IUnitOfWork.cs
│   └── UnitOfWork.cs
├── Configurations/
│   ├── UserConfiguration.cs
│   └── [others as needed]
└── SpreadsheetUtility.Library.DataAccess.csproj

SpreadsheetUtility.Library.Identity/
├── Models/
│   └── ApplicationUser.cs
├── Services/
│   ├── IAuthenticationService.cs
│   ├── AuthenticationService.cs
│   └── [others as needed]
├── DTOs/
│   ├── LoginRequest.cs
│   ├── RegisterRequest.cs
│   ├── AuthenticationResponse.cs
│   └── UserDto.cs
└── SpreadsheetUtility.Library.Identity.csproj

Tests (new test classes):
├── AuthenticationServiceTests.cs
├── UserRepositoryTests.cs
└── UnitOfWorkTests.cs

Configuration:
├── appsettings.json (updated)
├── appsettings.Development.json (new)
└── appsettings.Production.json (new)
```

---

## Estimated Timeline

| Task | Time | Cumulative |
|------|------|-----------|
| Setup Projects | 1 hour | 1 hour |
| Create Models | 1 hour | 2 hours |
| Implement Repositories | 1.5 hours | 3.5 hours |
| Implement Services | 1 hour | 4.5 hours |
| Write Tests | 1 hour | 5.5 hours |
| Database Setup & Migration | 30 min | 6 hours |
| Documentation & Polish | 30 min | 6.5 hours |

**Total**: ~6-7 hours (can be split across sessions)

---

## Resources Reference

### During Implementation
- [QUICK_REFERENCE.md](QUICK_REFERENCE.md) - Commands and patterns
- [ARCHITECTURE.md](ARCHITECTURE.md) - Design pattern details
- [.copilot-instructions](.copilot-instructions) - Code standards
- [IMPLEMENTATION_ROADMAP.md](IMPLEMENTATION_ROADMAP.md) - Step details

### For Specific Topics
- **EF Core Migrations**: QUICK_REFERENCE.md - Database section
- **Repository Pattern**: ARCHITECTURE.md or IMPLEMENTATION_ROADMAP.md
- **Unit Testing**: CONTRIBUTING.md - Testing section
- **Code Style**: .copilot-instructions or CONTRIBUTING.md

---

## Communication During Implementation

**Use these markers in commit messages**:
- `feat:` - New feature
- `refactor:` - Code restructuring
- `test:` - Tests added
- `docs:` - Documentation updates
- `fix:` - Bug fixes

**Example**:
```bash
git commit -m "feat: implement repository pattern for data access

- Create IGenericRepository interface
- Implement GenericRepository base class
- Add UserRepository for specialized queries
- All public methods are async"
```

---

## Post-Implementation

After completing Phase 1A:

1. **Code Review**: Request review from team
2. **Merge**: Merge to main branch
3. **Next**: Begin Phase 1B - Blazor Integration
4. **Update**: Update PROJECT_ENHANCEMENT_SUMMARY.md with completion

---

## Quick Start Command

```bash
# Start fresh build
dotnet clean
dotnet restore
dotnet build

# Create migration
dotnet ef migrations add InitialCreate -p SpreadsheetUtility\SpreadsheetUtility.Library.DataAccess

# Apply migration
dotnet ef database update -p SpreadsheetUtility\SpreadsheetUtility.Library.DataAccess

# Run tests
dotnet test

# Build & publish
dotnet publish -c Release
```

---

## Questions Before Starting?

Refer to:
- [CONTRIBUTING.md](CONTRIBUTING.md) - Contribution process
- [QUICK_REFERENCE.md](QUICK_REFERENCE.md) - Common commands
- [ARCHITECTURE.md](ARCHITECTURE.md) - Design patterns
- [.copilot-instructions](.copilot-instructions) - Standards

---

## ✅ Ready to Start?

- [ ] All documentation reviewed
- [ ] Environment is set up (VS, .NET 9 SDK)
- [ ] Project cloned and built successfully
- [ ] Checklist understood

**Status**: 🟢 Ready for Phase 1A Implementation

---

**Created**: [Current Session]  
**For**: Next development session  
**Expected Duration**: 6-7 hours  
**Estimated Completion**: 1-2 sessions

Good luck! 🚀
