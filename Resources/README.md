# Resources Folder Structure

This folder contains all the visual resources for the Stock Room application.

## Folder Organization

### Icons/
Store all application icons here:
- **App icons**: Main application icon (.ico format)
- **Form icons**: Icons for different forms (.png, .jpg, .ico)
- **Feature icons**: Icons representing different features

#### Icons/buttons/
Store button icons here:
- **Action buttons**: Add, Edit, Delete, Save, Cancel
- **Navigation buttons**: Back, Forward, Home
- **Tool buttons**: Search, Filter, Export

### Images/
Store larger images here:
- **Logos**: Company logos, branding images
- **Backgrounds**: Form backgrounds, splash screens
- **Graphics**: Charts, diagrams, decorative elements

## File Naming Conventions

Use descriptive, lowercase names with hyphens:
- `app-icon.ico` - Main application icon
- `login-form-icon.png` - Login form icon
- `add-button.png` - Add button icon
- `company-logo.png` - Company logo

## Supported Formats

- **Icons**: .ico, .png, .jpg, .gif
- **Images**: .png, .jpg, .bmp, .gif

## Usage in Code

Use the ResourceManager class to load resources:

```csharp
// Load form icon
this.Icon = ResourceManager.GetIcon("login-form-icon.ico");

// Load button image
addButton.Image = ResourceManager.GetButtonIcon("add-button.png");

// Load background image
this.BackgroundImage = ResourceManager.GetImage("background.jpg");
```

## Best Practices

1. **Optimize file sizes** - Compress images appropriately
2. **Use appropriate formats** - .ico for icons, .png for transparency
3. **Consistent sizing** - Use standard icon sizes (16x16, 32x32, 48x48)
4. **Descriptive names** - Use clear, descriptive file names
5. **Backup resources** - Keep source files in version control