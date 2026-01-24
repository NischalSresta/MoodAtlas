# MoodAtlas - Quick Start Guide

## ?? Getting Started

### First Time Setup

1. **Run the Application**
   ```bash
   dotnet run
   ```

2. **Create Your Account**
   - Navigate to `/create-user`
   - Enter username (min 3 characters)
   - Set security PIN (4-10 digits)
   - Choose theme preference
   - Click "Create User"

3. **Login**
   - Navigate to `/login`
   - Enter your username and PIN
   - Click "Login"
   - You'll be redirected to your dashboard

---

## ?? Main Features

### 1. Dashboard (`/dashboard-enhanced/{userId}`)
**Your Central Hub**
- View current journaling streak
- See mood distribution
- Check most used tags
- Quick access to all features
- Export your journal to PDF

**Quick Actions:**
- ?? Write Today's Entry
- ?? View Analytics
- ?? Open Calendar
- ?? Export Data

---

### 2. Create Entry (`/entry-editor`)
**Capture Your Day**

**What to Include:**
1. **Title**: Brief summary of your entry
2. **Mood**: Select one or more moods
   - Positive (Happy, Excited, Relaxed, etc.)
   - Neutral (Calm, Thoughtful, Curious, etc.)
   - Negative (Sad, Angry, Stressed, etc.)
3. **Content**: Write your thoughts and reflections
4. **Tags**: Add relevant tags (Work, Family, Exercise, etc.)
5. **Category**: Optional categorization

**Tips:**
- Select multiple moods if needed
- Mark one mood as primary
- Use tags for easy searching later
- Write freely - it's your private space

---

### 3. Browse Entries (`/entries`)
**Review Your Journey**

**Features:**
- ?? Search by title or content
- ?? Filter by date range
- ?? Paginated view
- ?? Click to edit any entry

**Search Tips:**
- Use keywords from title or content
- Set date range for specific periods
- Combine search and filters

---

### 4. Calendar View (`/calendar`)
**Visual Overview**

**Features:**
- Monthly calendar view
- See entries at a glance
- Mood indicators on dates
- Quick entry creation

**Navigation:**
- ? Previous / Next ? buttons
- "Today" button to return to current month
- Click any date to view/create entry

**Visual Indicators:**
- ?? Has entry
- ?? Entry with mood
- ?? Positive mood
- ?? Neutral mood
- ?? Negative mood

---

## ?? Common Tasks

### Writing Your First Entry
1. Click "Write Today's Entry" from dashboard
2. Or navigate to `/entry-editor`
3. Add a title
4. Select at least one mood
5. Write your thoughts
6. Add tags (optional)
7. Click "Save Entry"

### Editing an Entry
1. Go to `/entries`
2. Click on the entry you want to edit
3. Make your changes
4. Click "Update Entry"

### Viewing Your Progress
1. Go to dashboard
2. Check your streak stats
3. View mood distribution chart
4. See your most-used tags

### Exporting Your Journal
1. From dashboard, click "Export to PDF"
2. Select date range
3. Choose options (include moods/tags)
4. Click "Export"
5. PDF downloads automatically

---

## ?? Customization

### Theme
- Toggle between Light ?? and Dark ?? modes
- Click the theme button in navigation tabs
- Preference saves automatically

### Categories (Optional)
- Create custom categories
- Assign colors and icons
- Organize entries your way

### Custom Tags
- Add personalized tags
- Track recurring themes
- Improve search and filtering

---

## ?? Understanding Your Data

### Streak System
- **Current Streak**: Consecutive days with entries
- **Longest Streak**: Best achievement to date
- **Missed Days**: Days without entries in selected period

### Mood Tracking
- **Distribution**: Percentage breakdown (Positive/Neutral/Negative)
- **Most Frequent**: Your most common mood state
- **Trends**: Visual charts show patterns over time

### Analytics
- **Word Count**: Average and trends
- **Tag Frequency**: Most-used tags
- **Consistency Score**: Entry regularity
- **Insights**: AI-generated (coming soon)

---

## ?? Security & Privacy

### Your Data is Safe
- ? All data stored locally
- ? PIN-protected access
- ? No cloud sync (fully offline)
- ? Export your data anytime

### Best Practices
1. Choose a strong, memorable PIN
2. Regularly export backups
3. Keep your device secure
4. Don't share your PIN

---

## ?? Tips for Success

### Journaling Tips
1. **Be Consistent**: Write daily, even if brief
2. **Be Honest**: This is your private space
3. **Reflect**: Review past entries for insights
4. **Track Patterns**: Notice mood trends
5. **Celebrate Progress**: Check your streak!

### App Usage Tips
1. **Use Tags**: Makes searching easier
2. **Try Filters**: Find specific memories
3. **Check Calendar**: Visual overview helps
4. **Export Regularly**: Create backups
5. **Explore Moods**: Use variety for accuracy

---

## ? Troubleshooting

### Can't Login?
- Check username spelling
- Verify PIN is correct
- If forgotten, create new user (data separate)

### Entry Not Saving?
- Ensure title and content filled
- Select at least one mood
- Check for error messages

### Navigation Issues?
- Refresh the page
- Check you're logged in
- Clear browser cache if needed

---

## ??? Quick Navigation Reference

| From | To | Click |
|------|----|-

---|
| Anywhere | Dashboard | "Dashboard" tab |
| Anywhere | New Entry | "New Entry" tab |
| Anywhere | Entries List | "Entries" tab |
| Anywhere | Calendar | "Calendar" tab |
| Dashboard | New Entry | "Write Today's Entry" button |
| Entries | Edit Entry | Click any entry |
| Calendar | Date Entry | Click any date |

---

## ?? Keyboard Shortcuts (Coming Soon)

- `Ctrl+N`: New entry
- `Ctrl+S`: Save entry
- `Ctrl+/`: Search entries
- `Esc`: Close modals

---

## ?? Learn More

### Advanced Features
- Custom categories
- Tag management
- Data export options
- Analytics deep-dive

### Best Practices
- [Journaling Guide](#) (coming soon)
- [Mood Tracking Tips](#) (coming soon)
- [Mental Health Resources](#) (coming soon)

---

## ?? Support

### Having Issues?
1. Check this guide
2. Review `NAVIGATION_GUIDE.md`
3. Check browser console for errors
4. Verify you're using latest version

### Feature Requests
- Open an issue on GitHub
- Describe your use case
- Suggest improvements

---

## ?? Ready to Start!

**Your Journey Begins:**
1. ? Create account (`/create-user`)
2. ? Login (`/login`)
3. ? Write first entry (`/entry-editor`)
4. ? Track your mood
5. ? Build your streak
6. ? Reflect and grow

**Happy Journaling! ???**

---

**Version**: 1.0  
**Last Updated**: January 2026  
**Build Status**: ? Stable
