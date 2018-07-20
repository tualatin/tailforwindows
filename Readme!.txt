.NET 4.x required!

Source: git@github.com:tualatin/tailforwindows.git
Known issues: https://github.com/tualatin/tailforwindows/issues

If there are problems, Tail4Windows creates log files in folder 'logs'. You can send me the log file from the day where the error occurred. I will investigate it and fix it as soon as possible.

Take a look to the Tail4Windows Wiki page, if you want more informations: https://github.com/tualatin/tailforwindows/wiki

Changelog:
v2.0.xxxx.x
* bug fix DataGrid style
* bug fix find algorithm
* bug fix highlight data
* bug clear log items -> Statusbar does not refresh

v.2.0.6769.x
* new add selected word to Filter/HighlightManager
* new user can delete FindWhat history
* new keyword highlighting
* new algorithm to read log files (Tail4Windows does not block files anymore)
* new read Windows event log
* new SplitWindow view
* new Drag'n'Drop window design
* new Tabheaders can colorized by user
* new undo manager
* new Find result window
* new SmartWatch can use Wildcards
* new SmartWatch interval is configurable (min 2sec, max 1 min)
* new when no pattern given in SmartWatch, it tries to find out the right pattern with internal logic
* new SmartWatch can used with non saved items
* new Quick save
* new Quick add
* new Logfile history
* new delete Logfile history
* new Database engine
* new Regex helper
* new Statusbar design
* new per user settings -> Users\user\AppData\Roaming\Tail4Windows\
* new translateable userinterface - english, german
* new filters can be enabled/disabled
* it's faster than version 1.5.xxxx
* bug fix search highlighting
* bug fix filtering
* bug fix time settings
* bug fix date settings
* remove some old settings from config file -> Tail4Windows will check it at startup
* complete redesign of FindWhat
* redesign of Pattern control
* add more shortcuts -> see Shortcuts.txt
* add more Options
* new Icon design
* FileManager is TailManager (complete redesign) now
* Filters is FilterManager (complete redesign) now
* complete new Options design
* complete Code redesign

v1.5.6312.x
* add SmartWatch
* add Log line limitation, if needed
* add some default patterns
* add pattern logic to FileManager
* history saves content in own XML file
* add validation in Filter view and FileManager
* lot of bug fixes
* new design for all main controls
* Group by Category option in FileManager
* Go forward in  search dialog is by key F3 possible now
* SpinnerControl accept MouseWheel now
* File encoding can change while tailing
* new Scrollbar design
* improve Drag'n'Drop behaviour
* change internal structure 
* bug fix auto update
* change some icons
* Tail4Windows log files older than 5 days will automatically delete, if you don't want it, please edit the config file and set property 'DeleteLogFiles' to 'False'
* new log4net is logging system now
* new tooltip design
* bug fix Uptime
* improve StatusBar Tooltip information
* some code refactories

v1.0.5400.x
* bug fix FileManager, does not changed the FontType
* bug fix get file size exception
* bug fix select first category when add file from main window to FileManager
* bug fix FileManager sort alpha alphabetical order
* bug fix FileManager update Thread Priority, Refresh Rate changes in datagrid in realtime
* update ErrorLog-Controller, actual date will log now
* bug fix add new filter from main window
* add option command parameter to add file to TfW
* add add/remove SendTo menu
* bug fix can not delete or edit logfile after TfW is stopped

v1.0.5296.x
* bug fix searching for bookmark lines
* bug fix save search box position
* bug fix keyboard navigation
* bug fix unhandle exception when open log file properties from FileManager
* improve tab control handler
* improve tail thread
* add mouse double click to open file from FileManager
* save if Filter is on/off
* click mousewheel add a new tab
* click mousewheel on tab, remove this tab
* some other bug fixes

v0.9.5231.x
* bug fix when clear FileName textbox, than press Start, textbox was empty
* add select single word
* add multiselect by mouse
* improve performance multiselect by mouse
* bug fix mouse double click on a empty text editor window
* bug fix rightmouse buttondown by multiselect items
* bug fix NullReferenceException when closing TfW
* add popup window alert option
* improve spinner control: hold mouse button down increment or decrement value
* bug fix spinner control
* new IconTray API
* bug fix when TextBox FileName has focus, user type a "t" or "f" -> "Always on top" or "Filter" toggle on/off
--- Hint ---
Please insert the following lines into your config file, when you use TailForWindows in the past:
<add key="Alert.PopupWindow" value="false" />

v0.9.5172.x
* bug fix proxy authentication
* bug fix drag and drop while a process is running
* change description text box in FileManager
* change category text box in FileManager
* add TLS/SSL support for SMTP
* add E-Mail options in Alert option tab
* add SMTP settings dialogue
* add SMTP settings
* improve read settings
* change Filters icon
* bug fix exception when enter a E-Mail address in Alert dialogue
* remove systeminformation from TabItem (improve speed to call settings dialogue)
* add systeminformation dialogue in About-Tab
* add import and export settings
* add autoupdate
* improve updater
* bug fix proxy settings
* add bookmark option
* add "Search Bookmarks" in find box
* add contextmenu for Bookmark lines to remove all Bookmarks
* bug fix find what window title
* bug fix in FileManager
* bug fix FileManager add file

v0.9.5099.x
* add shortcut Ctrl+Alt+M minimize main window
* add shortcut F (Filter on/off)
* add shortcut T (Always on top on/off)
* FileManager dialogue add Enter key event
* GoToLine dialogue add press Enter key event
* add watermark textbox
* change tab layout
* update tab layout
* fix problem with Ctrl+Tab
* shortcut Ctrl+T change to new tab window
* add shortcut Ctrl+W close tab window
* change shortcut Start tail (new Ctrl+R)
* fix problems with linenumber font
* add Webside information in About dialogue
* add shortcut CTRL+G (GoToLine)
* add GoToLine
* add sound support
* bug fix loading soundfile
* fix problems with FontType in TextEditor
* bug fix font change in FileManager
* bug fix in FilterDialogue change event
* bug fix change FontType
* bug fix in FileManager
* bug fixes writing XML file
* add history for search dialogue
* rename "Filters active" -> "Filter"
* "Add" button not enabled again when file is open from FileManager

v0.9.5087.x
* stability improvements
* add filtering in view
* improve tab selection change (minimize event firing)
* lot bug fixes in search dialogue for more than one tab page
* bug fix search next - scroll from beginning
* bug fix for Windows XP
* replace some icons (bug fix for Windows XP)
* add Drag and Drop support
* add Drag and Drop support in FileManager
* add alert tab in options dialogue
* add more options in *.config file
* add filters
* add alert support
* add read filter section from XML file
* add writing filter section into XML file
* bug fix in FileManager
* bug fix Count button in search dialogue
* rename button "Manager font" -> "Manage font"
* improve Find next in search dialogue

v0.9.5078.x
* sorry the Filtersection is not implemented now, it comes as soon as possible!
* add some shortcut keys (Ctrl+F - SearchDialog, Ctrl+O - OpenFileDialog, Ctrl+M - FileManagerDialog, Ctrl+E - Clears all content in TextEditor, Ctrl+T - Start tail, Ctrl+S - Pause tail)
* improve button behaviour Start/Stop
* add searchbox
* add safe search words until sessions end
* bug fix scroll problems
* improve CPU usage
* improve scroll behaviour
* improve read first n lines
* new datacollection (better performance)
* change WPF TextBox to new TextEditorControl (Search highlighting, Linenmumbers) - it was a lot of work!
* add printer support
* add ShowLineNumbers as option
* add line numbers colour option
* new print page format
* add new option FileManagerSort (Nothing - no sort, FileCreationTime - sort by file creation time)
* new property FileAge
* new property FileCreationTime
* rename status bar message ("Record" to "Tail")
* remove "copy to clipboard" button, was useless
* bug fix search dialogue title
* bug fix refresh datagrid source

v0.9.5049.x
* colour picker to customize colour in tail window
* better event handling in all dialogues for Escape button
* improve file open event from FileManager dialogue
* improve file open event from FileManager dialogue to open a new program instance
* add more settings options in .config file
* rename FindHighlightColor to FindHightlightBackgroundColor/add FindHighlightForegroundColor
* remove file sort option from Options dialogue because WPF datagrid do it better for us
* add x64 Bit support
* bug fix computing free available memory in systeminformation dialogue
* bug fix timestamp