# Example Files Download - User Guide

## 📥 Overview

The Example Files feature allows you to download sample spreadsheets that demonstrate the required format for using the Gantt Chart Generator. This helps you quickly get started without having to create files from scratch.

## 🎯 Getting Started

### Accessing Example Files

1. **From the UI:**
   - Click **"Example Files"** in the navigation menu
   - Or visit: `https://yourapp.com/example-files`

2. **From the API:**
   - List files: `GET /api/examplefiles`
   - Download: `GET /api/examplefiles/{fileName}`

### Browsing Files

The Example Files page displays:
- **File Name**: The name of the example file
- **Description**: What the file demonstrates
- **Size**: File size in KB/MB
- **Last Modified**: When the file was last updated
- **Download Button**: Click to download the file

## 📥 Downloading Files

### Step-by-Step

1. Navigate to `/example-files`
2. Locate the file you want to download
3. Click the **"⬇️ Download"** button
4. Your browser will download the file
5. The file is now ready to use

### What You Get

- Excel spreadsheet (`.xlsx` format)
- Pre-formatted with example data
- Ready to use as a template
- Shows proper structure for Gantt charts

## 🔍 File Details

Each example file demonstrates:
- Required column structure
- Data format for different fields
- How to organize projects, tasks, and team members
- Best practices for data entry

### Common File Names

- `SampleProject.xlsx` - Basic project example
- `MultiProjectExample.xlsx` - Multiple projects
- `AdvancedTeamScheduling.xlsx` - Complex scheduling
- `DevelopmentProject.xlsx` - Software development example

## 💡 How to Use Downloaded Files

1. **Review the structure** - Open the file and examine the columns
2. **Understand the format** - See how data is organized
3. **Create your own** - Follow the same structure for your data
4. **Upload/Paste** - Use the Gantt Generator to process your data

### Next Steps After Download

1. Go to [Gantt Generator](/ganttGeneratorFromPaste)
2. Create your own file following the example structure
3. Paste or upload your data
4. Generate your Gantt chart

## 🛠️ Using with Gantt Generator

### Workflow

```
Download Example
      ↓
Review Format
      ↓
Create Your File
      ↓
Use Gantt Generator
      ↓
Generate Chart
```

### Paste Method

1. Download an example file
2. Copy data from the example
3. Modify to match your projects
4. Paste into Gantt Generator
5. Click "Generate Chart"

## ❓ Frequently Asked Questions

### Q: What format are the files in?
**A:** All files are in Excel format (`.xlsx`). You can open them with:
- Microsoft Excel
- Google Sheets
- LibreOffice Calc
- Any spreadsheet application

### Q: Can I modify the downloaded files?
**A:** Yes! The files are yours to modify. You can:
- Add your own data
- Remove example data
- Restructure as needed
- Save as your own template

### Q: Are there different file types?
**A:** Yes! Each file demonstrates different scenarios:
- Simple projects (1-2 projects)
- Complex projects (multiple teams, dependencies)
- Different industries (software, construction, etc.)

### Q: How do I know which file to use?
**A:** Choose based on your use case:
- **Simple project?** → Start with basic example
- **Multiple teams?** → Use team scheduling example
- **Complex dependencies?** → Use advanced example

### Q: Can I use the example files directly?
**A:** You can test with them directly, but typically you'll:
1. Download the example
2. Replace data with your own
3. Keep the same structure
4. Generate your chart

## 🔗 Related Resources

- [Gantt Generator](/ganttGeneratorFromPaste) - Create your charts
- [Home](/) - Main application
- [JSON Generator](/jsonGeneratorFromPaste) - Alternative data format

## ⚠️ Troubleshooting

### File Won't Download
- Check your browser settings
- Try a different file
- Check internet connection
- Clear browser cache

### Can't Open Downloaded File
- Install Excel or compatible application
- Check file isn't corrupted
- Try opening in different application
- Re-download the file

### File Format Not Recognized
- Ensure file is `.xlsx` format
- Check file extension in download dialog
- Save with correct extension

## 📞 Need Help?

- Review the file content carefully
- Check data types match the examples
- Verify column names are exact matches
- Contact support if issues persist

## ✅ Checklist Before Using

- [ ] Downloaded the example file
- [ ] Opened the file successfully
- [ ] Reviewed the column structure
- [ ] Understood the data format
- [ ] Ready to create your own file

---

**Status:** Ready to use ✅

For technical details, see [EXAMPLE_FILES_IMPLEMENTATION.md](./EXAMPLE_FILES_IMPLEMENTATION.md)
