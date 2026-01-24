# MoodAtlas Navigation Guide

## ??? Application Routes & Navigation Structure

### Main Application Routes

| Route | Page | Description | Access Level |
|-------|------|-------------|--------------|
| `/` | Home | Redirects to Login | Public |
| `/login` | Login | User authentication | Public |
| `/create-user` | Create User | New user registration | Public |
| `/dashboard/{UserId}` | Dashboard | Basic dashboard | Authenticated |
| `/dashboard-enhanced/{UserId}` | Enhanced Dashboard | Advanced analytics dashboard | Authenticated |
| `/entry-editor` | New Entry | Create new journal entry | Authenticated |
| `/entry-editor/{EntryId}` | Edit Entry | Edit existing entry | Authenticated |
| `/entries` | All Entries | List all journal entries | Authenticated |
| `/calendar` | Calendar View | Monthly calendar with entries | Authenticated |

---

## ?? Navigation Menu Structure

### Main Navigation (NavMenu.razor)
Located in the left sidebar:

1. **Login** (`/login`) - User login page
2. **Create User** (`/create-user`) - New user registration
3. **Dashboard** (`/dashboard-enhanced/1`) - Main dashboard
4. **New Entry** (`/entry-editor`) - Create journal entry
5. **All Entries** (`/entries`) - Browse all entries
6. **Calendar** (`/calendar`) - Calendar view

### In-Page Navigation Tabs
Most authenticated pages include navigation tabs:

```razor
<div class="nav nav-tabs mb-4">
    <a href="/dashboard-enhanced/@UserId">Dashboard</a>
    <a href="/entry-editor">New Entry</a>
    <a href="/entries">Entries</a>
    <a href="/calendar">Calendar</a>
    <button @onclick="ToggleTheme">??/??</button>
</div>
```

---

## ?? Navigation Flows

### 1. User Registration Flow
```
/ ? /login ? /create-user ? /login ? /dashboard-enhanced/{UserId}
```

### 2. Login Flow
```
/ ? /login ? /dashboard-enhanced/{UserId}
```

### 3. Entry Creation Flow
```
/dashboard-enhanced/{UserId} ? /entry-editor ? Save ? /entries
```

### 4. Entry Editing Flow
```
/entries ? Click Entry ? /entry-editor/{EntryId} ? Save ? /entries
```

### 5. Calendar Navigation Flow
```
/calendar ? Click Date ? /entry-editor?date={date}
```

---

## ?? Key Navigation Patterns

### Dashboard Navigation
From Dashboard, users can:
- Click "Write Today's Entry" ? `/entry-editor`
- Click "Export to PDF" ? Opens export modal
- Click "View Calendar" ? `/calendar`
- Click "Logout" ? `/login`

### Entries Page Navigation
From Entries page, users can:
- Click "New Entry" button ? `/entry-editor`
- Click on any entry ? `/entry-editor/{EntryId}`
- Search and filter entries
- Navigate between pages

### Calendar View Navigation
From Calendar, users can:
- Click on a date with entry ? View/Edit entry
- Click "Today" button ? Returns to current month
- Navigate between months
- Click "Dashboard" ? Returns to dashboard

---

## ?? Authentication Guards

All pages except `/`, `/login`, and `/create-user` require authentication:

```csharp
protected override async Task OnInitializedAsync()
{
    var users = await UserService.GetAllUsersAsync();
    currentUser = users.FirstOrDefault();

    if (currentUser == null)
    {
        Navigation.NavigateTo("/login");
        return;
    }
    // ... rest of initialization
}
```

---

## ??? Navigation Helper Methods

### Common Navigation Methods

```csharp
// Navigate to login
private void Logout()
{
    Navigation.NavigateTo("/login");
}

// Navigate to dashboard
private void GoToDashboard()
{
    Navigation.NavigateTo($"/dashboard-enhanced/{currentUser.UserId}");
}

// Navigate to new entry
private void CreateNewEntry()
{
    Navigation.NavigateTo("/entry-editor");
}

// Navigate to edit entry
private void EditEntry(int entryId)
{
    Navigation.NavigateTo($"/entry-editor/{entryId}");
}

// Navigate to entries list
private void GoToEntries()
{
    Navigation.NavigateTo("/entries");
}

// Navigate to calendar
private void GoToCalendar()
{
    Navigation.NavigateTo("/calendar");
}
```

---

## ?? Navigation Icons (Bootstrap Icons)

- **Login**: `bi-box-arrow-in-right`
- **Dashboard**: `bi-speedometer2`
- **New Entry**: `bi-pencil-square`
- **Entries**: `bi-journal-text`
- **Calendar**: `bi-calendar3`
- **User**: `bi-person-plus`
- **Export**: `bi-download`
- **Delete**: `bi-trash`

---

## ?? Quick Reference

### Starting the App
1. App starts at `/` (Home)
2. Automatically redirects to `/login`
3. After login, redirects to `/dashboard-enhanced/{UserId}`

### Creating First Entry
1. From dashboard: Click "Write Today's Entry"
2. Or use nav: Click "New Entry"
3. Fill form ? Save ? Redirects to `/entries`

### Viewing Past Entries
1. Navigate to `/entries`
2. Use search/filter to find entries
3. Click entry to edit

### Calendar View
1. Navigate to `/calendar`
2. View entries by month
3. Click date to create/edit entry

---

## ?? Navigation Best Practices

1. **Always use `Navigation.NavigateTo()` for programmatic navigation**
2. **Use `<a href="">` for static links in markup**
3. **Include userId in dashboard routes**: `/dashboard-enhanced/{userId}`
4. **Pass parameters in URLs**: `/entry-editor/{entryId}`
5. **Check authentication before rendering sensitive data**
6. **Provide clear back navigation options**
7. **Show loading states during navigation**

---

## ?? Navigation Updates Applied

### Fixed Routes
- ? Standardized entry editor route: `/entry-editor/{EntryId?}`
- ? Updated dashboard route: `/dashboard-enhanced/{UserId}`
- ? Fixed calendar navigation
- ? Corrected entries page links
- ? Updated login redirect

### Updated Navigation Menus
- ? NavMenu.razor - Main sidebar navigation
- ? Dashboard.razor - In-page tabs
- ? Entries.razor - In-page tabs
- ? Calendar.razor - Header navigation

### Navigation Improvements
- ? Consistent route naming across all pages
- ? Proper parameter passing for user-specific content
- ? Fixed broken links and redirects
- ? Added theme toggle buttons in navigation
- ? Improved logout functionality

---

## ?? Support

For navigation issues:
1. Check if user is authenticated
2. Verify userId is being passed correctly
3. Ensure route parameters match page definitions
4. Check browser console for navigation errors
5. Verify all links use correct route patterns

---

**Last Updated**: January 2026
**Version**: 1.0
**Build Status**: ? Successful
