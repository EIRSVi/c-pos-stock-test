# FontAwesome Icon Mapping Configuration

## FontAwesome 7.0.0 Icon Directory Structure
Your FontAwesome directory: `X:\c\resources\icon\fontawesome-free-7.0.0-desktop`

### Expected Directory Structure:
```
fontawesome-free-7.0.0-desktop/
??? svgs/
?   ??? solid/        # Filled icons
?   ??? regular/      # Outlined icons
?   ??? brands/       # Brand icons
??? webfonts/         # Font files
??? css/              # CSS files
```

### Icons Used in Dashboard:

#### Navigation Icons:
- **Dashboard/Home**: `house.svg` or `home.svg` (solid)
- **Inventory/Stock**: `boxes-stacked.svg` or `boxes.svg` (solid)
- **Reports/Analytics**: `chart-column.svg` or `chart-bar.svg` (solid)  
- **Settings**: `gear.svg` or `cog.svg` (solid)

#### Header Icons:
- **App Icon**: `warehouse.svg` or `building.svg` (solid)
- **User**: `user.svg` (solid)
- **Logout**: `right-from-bracket.svg` or `sign-out-alt.svg` (solid)

### Icon File Names to Check:
Look for these files in your `X:\c\resources\icon\fontawesome-free-7.0.0-desktop\svgs\solid\` folder:

1. `house.svg` (Home/Dashboard)
2. `boxes-stacked.svg` (Inventory)
3. `chart-column.svg` (Reports)
4. `gear.svg` (Settings)
5. `warehouse.svg` (App icon)
6. `user.svg` (User)
7. `right-from-bracket.svg` (Logout)

### Alternative Icon Names:
If the above don't exist, try these alternatives:
- Home: `home.svg`
- Inventory: `boxes.svg`, `archive.svg`
- Reports: `chart-bar.svg`, `chart-line.svg`
- Settings: `cog.svg`, `wrench.svg`
- App: `building.svg`, `store.svg`
- Logout: `sign-out-alt.svg`, `door-open.svg`

### Customization:
To change icons, edit the `ResourceManager.DashboardIcons` class methods and update the icon file names to match your FontAwesome files.

### Color Scheme:
- **Active Icon Color**: White (#FFFFFF)
- **Inactive Icon Color**: Light Gray (#E2E8F0)
- **Primary Blue**: #2563EB
- **Success Green**: #22C55E
- **Warning Orange**: #F59E0B
- **Error Red**: #EF4444

### Icon Sizes:
- **Button Icons**: 24x24px
- **User Icon**: 16x16px
- **App Icon**: 32x32px