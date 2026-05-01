# Phase 2: Blazor Authentication UI Components - Implementation Plan

## Overview
Phase 2 will create the complete Blazor UI for authentication, including:
- Login component
- Register component  
- Custom authentication state provider
- Protected layout with user info display
- Navigation menu with auth-aware items

---

## Components to Create

### 1. CustomAuthenticationStateProvider.cs
**Location**: `SpreadsheetUtility.UI.Web/Services/CustomAuthenticationStateProvider.cs`

**Purpose**: Provides authentication state to Blazor components

**Key Methods**:
- `GetAuthenticationStateAsync()`: Returns current user's claim principal
- `NotifyUserAuthentication(string email)`: Updates state after login
- `NotifyUserLogout()`: Clears state after logout

**Integration**: 
- Register in Program.cs as scoped service
- Uses local storage or session to persist login state

---

### 2. Login.razor Component
**Location**: `SpreadsheetUtility.UI.Web/Components/Pages/Authentication/Login.razor`

**Features**:
- Email input field
- Password input field
- Remember Me checkbox
- Login button with loading state
- Error message display
- Link to register page
- Redirect to home on successful login

**Logic**:
```
1. Validate form inputs
2. Call IAuthenticationService.LoginAsync()
3. If success:
   - Store user token/session
   - Update authentication state
   - Redirect to home page
4. If failed:
   - Display error messages
```

---

### 3. Register.razor Component
**Location**: `SpreadsheetUtility.UI.Web/Components/Pages/Authentication/Register.razor`

**Features**:
- Email input field
- Password input field
- Confirm Password field
- First Name input (optional)
- Last Name input (optional)
- Register button with loading state
- Error message display
- Link to login page
- Success message + redirect

**Logic**:
```
1. Validate form inputs (especially password match)
2. Call IAuthenticationService.RegisterAsync()
3. If success:
   - Show success message
   - Auto-login OR redirect to login
4. If failed:
   - Display validation errors
```

---

### 4. AuthorizedLayout.razor Component
**Location**: `SpreadsheetUtility.UI.Web/Components/Layout/AuthorizedLayout.razor`

**Features**:
- Show MainLayout only if user is authenticated
- Display user info (email, full name)
- Logout button
- Redirect to login if not authenticated

**Usage**: Wrap protected pages with this layout

---

### 5. AuthorizeView Integration
**Updates to MainLayout.razor**:
- Add user greeting display
- Show login/logout buttons based on auth state
- Update NavMenu to show auth status

---

### 6. Updated NavMenu.razor
**Changes**:
- Show menu items only for authenticated users
- Display current user email
- Add Logout button
- Add Login/Register links for unauthenticated users

---

## Implementation Steps

### Step 1: Create Authentication Folder Structure
```
SpreadsheetUtility.UI.Web/
├── Components/
│   ├── Pages/
│   │   └── Authentication/
│   │       ├── Login.razor
│   │       ├── Login.razor.cs (code-behind)
│   │       ├── Register.razor
│   │       └── Register.razor.cs (code-behind)
│   └── Layout/
│       └── AuthorizedLayout.razor
├── Services/
│   └── CustomAuthenticationStateProvider.cs
```

### Step 2: Create CustomAuthenticationStateProvider
- Implement AuthenticationStateProvider base class
- Add user state management
- Integrate with local storage

### Step 3: Create Login Component
- Build form with validation
- Integrate IAuthenticationService
- Add error handling
- Implement redirect logic

### Step 4: Create Register Component
- Build registration form
- Add password confirmation validation
- Integrate IAuthenticationService
- Add success/error handling

### Step 5: Update Layouts
- Update MainLayout with AuthorizeView
- Create AuthorizedLayout
- Update NavMenu with auth logic

### Step 6: Update Program.cs
- Register CustomAuthenticationStateProvider
- Add CascadingAuthenticationState
- Configure route/redirect behavior

### Step 7: Update _Imports.razor
- Add authentication namespace imports
- Add cascade component imports

### Step 8: Testing
- Test successful login
- Test failed login (invalid credentials)
- Test registration
- Test logout
- Test protected route access

---

## Routing Structure

```
/login           → Login.razor
/register        → Register.razor
/home            → Home.razor (protected)
/gantt/*         → All protected pages
```

---

## Security Considerations

1. **Token Management**: Store JWT in local storage or session cookie
2. **XSS Prevention**: Razor automatically escapes HTML output
3. **CSRF Protection**: ASP.NET Core Antiforgery middleware
4. **Password**: Never log or display passwords
5. **Session Timeout**: Consider implementing idle timeout
6. **Secure Flag**: Use HTTPS only in production

---

## Dependencies Required

```csharp
// Already available in SpreadsheetUtility.UI.Web
- Microsoft.AspNetCore.Components.Authorization
- Microsoft.AspNetCore.Blazor.LocalStorage (if using local storage)
```

---

## Phase 2 Checklist

- [ ] Create Authentication folder in Components/Pages
- [ ] Create CustomAuthenticationStateProvider
- [ ] Create Login.razor component
- [ ] Create Register.razor component
- [ ] Create AuthorizedLayout
- [ ] Update MainLayout with AuthorizeView
- [ ] Update NavMenu with auth-aware items
- [ ] Update Program.cs with auth services
- [ ] Update _Imports.razor
- [ ] Test all authentication flows
- [ ] Verify protected routes work
- [ ] Test logout functionality

---

## File Dependencies

```
_Imports.razor (update)
    ↓
Program.cs (update)
    ↓
CustomAuthenticationStateProvider.cs (create)
    ↓
Login.razor (create)
Register.razor (create)
    ↓
AuthorizedLayout.razor (create)
MainLayout.razor (update)
NavMenu.razor (update)
```

---

## Estimated Completion

- CustomAuthenticationStateProvider: 15 min
- Login Component: 20 min
- Register Component: 20 min
- Layout Updates: 15 min
- Program.cs Configuration: 10 min
- Testing: 20 min

**Total: ~100 minutes**

Ready to start Phase 2? 🚀
