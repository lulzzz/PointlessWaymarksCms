﻿## Todos
 - Points: With data structure change review JSON saving
 - Why is the top leaflet map popups getting the maxwidth in the GC Test Site? Load order? Delay loading until some initialized event?
 - Review Tags - could changes be reduced to 'primary' changes rather than all changes?
 - The GUI Context tests are breaking - longer waits needed for the background events to run? Something else?
 - Look again and Main Feed/Before/After for content - currently the entire main feed is regenerated - could this be done selectively and/or does other content need before/after help?
 - File Log Window:
  - DataNotification for Generation Log needs a quick test
  - The script written refreshes to the datetime filter should be via datanotifications not direct
  - Changing the datetime filter should run the report? maybe run report becomes refresh report or ...
  - Clearing Generation Log Entries needs DataNotification Support
 - Revisit AvalonEdit for the BodyContent Editor for performance reasons - probably 'as a service' since for perf AvalonEdit doesn't expose a bound Text property (maybe look at https://github.com/martinkirsche/AsYouTypeSpellChecker for spell checking)
 - Could Tag List javascript be abstracted to site resources easily?
 - Better loading indication on the search lists site pages
 - Spatial:
  - Bracket Codes - Points, GeoJson, Lines
  - GeoJson
  - Lines
 - Text to Speech:
   - Cancellation
   - in Update Notes
   - Potentially in editor
 - In the Conversion Data Entry Control the factory method should probably take the Conversion Function - the issue is that if you set user text and then the conversion function the user value may be unset - probably better than converting when the conversion function is set, or both?
 - Track tags in db so changes can be more quickly detected (long generation time on PointlessWaymarks.com after adding all photos)
 - Integrate Xaml Styler - is the git hook working? I don't think it is...
 - Check that items like the Menus and Excluded tags are saved to Json and are restored from Json
 - Deleted Content Report so it is possible to restore completely deleted
 - In Search it might be nice to have the content type on the line with date?
 - Could I successfully tuck away a copy of the current edits to help in unexpected shut downs? No good if I can't expose these in a helpful way...
 - Sorting needs better visual indicators
 - Should temp files be auto-deleted or live forever?
 - Folder Name in Lists needs to open in Explorer when clicked
 - Revisit og and dublin metadata - reference site not code and is it used correctly? Other tags that could be included?
 - Is everything getting HMTL Encoded - are there spots with leaks? (tests?)
 - RSS - Does title need CDATA to be completely safe? Or?
 - Figure out a system to allow StatusContext to help out positioning a new window vs the launch window

## Ideas
 - Could you write S3 object metadata to indicate file version in a way that would coordinate and be a quick check against local files?
 - Clean up the main window - split out context - consider creating a control?
 - Should the code record a checksum of media archive files to be able to check if they change on disk rather than thru the program?
 - Review https://github.com/Softwire/HighlightingTextBox/blob/master/HighlightingTextBox/HighlightingTextBox.cs - is there something useful here?
 - Clicking a content code should open the editor for that content? Maybe don't highlight/hint just process the selected text - would make hidden but useful and could maybe support urls from text as well - perhaps via Behavior?
 - History Cleanup for Content. Maybe on save - limit to ?last 1000?, it would be great to not grow to infinity but want to save very old entries for seldom edited content and many entries for frequently edited content...
 - GUI Automation Testing https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/march/test-run-automating-ui-tests-in-wpf-applications and/or ViewModel testing (the ViewModels should be testable without the views - however an interesting issue is that testing the GUI will test both...)
 - Look at deployment options - self contained? msix? automated?
 - Watch https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/ - the source generators look like they could be quite interesting for INPC and HasChanges
 - Provide a bit of abstraction to easily do MarkDown help text - see pdftocairo notes in settings - that works ok but font size don't match, link handler is code behind...
 - Could all font sizes be controlled by slider? I like the control in the editor but maybe everywhere would be more useful? And persist in Settings?
 - Explore https://wkhtmltopdf.org/ - Is this actually a jumping off point to an interlinked set of pdfs - maybe for some portion or subsection of the site - or maybe look for other PDF generation strategies
 - How hard would it be to create a GridSplitter that would be easy to set the initial split based off of the last editor use - and/or that could completely hide content to one side with a shortcut
 
 
## Notes

11/27/2020

Found that the Mapcomponent was picking up post p styling in the popup and tooltip bindings causing the usual quite nice autosizing of those to break. For now just went to the simplest possible text only popup.

11/25/2020

Happy Thanksgiving! Among many other things feeling thankful that I can continue to work on this project.

Map GUI editor is working - the current quick hack for adding elements is to allow pasting in ContentIds and then to process those - drag and drop is the eventual target here but I had issues with Gong Drag and Drop not adding thru nuget. I may have made more sense to work on GeoJson and Lines before this but I am anxious to use this feature on PointlessWaymarks.

Had to revert from the latest WebView2 - quickly ran into odd problems, good reminder about what to expect with pre-release projects.

Simplified the Point Data Structures after working on the MapComponent.

11/20/2020

First successful load of a map via bracket code in a Grand Canyon test post.

11/17/2020

With the uploader in a decent spot (and probably needing more usage to really figure out what would be the most valuable to add/change/fix) I started working on the 'map' feature for an upcoming Ironwwod Forest/Dos Titos post. Points, Lines and GeoJson are all both important enough to be treated as 'content'/a distinct unit - but also flexible enough to serve as a reasonably complete basis for showing a user maps. The idea with the maps is to give a way to associate sets of features into a map. Atm the idea is that maps will not actually be available as a 'page' or distinct type but only as a part of something else - and that the maps will depend on looking up on site json information to pull together the various elements.

11/12/2020

First upload batch tracking in uploads window and saving Json Uploads from the written files tab. Still quite a few todos but it continues to look like this will be a very useful project.

BindingProxy - A classic problem in WPF applications is that List Items have the data item as the DataContext and by default don't have a reference to the parent datacontext. This is an issue because while some commands make sense on a list item a 'delete item' type command is most easily done from the parent context. There are a number of possible solutions and in WPF for quite a long time I used a 'BindingProxy' class to create a static resource that list items could bind to - this works but in modern times, with good xaml intellisense and binding checks, it means that the datacontext type is unknown and doesn't take maximum advantage of the system... While watching a Brian Lagunas video I was reminded of a simpler tecnique that I have used extensively in Xamarin Forms - giving the top level control a name and doing an exlement binding - this works with intellisense and binding checks and in retrospect using the BindingProxy was really just habit from the past at this point. There probably are situations where the BindingProxy might be able to solve a problem when you don't have access to the Element Name of the parent - but this isn't coming up for me with the current project structure. (Btw my experience is that RelativeSource doesn't solve all problems (at least in a reasonable way) because of interesting situations like the tooltip that has it's own logical and visual tree...).

11/10/2020

The scripts to upload files to S3 have been quite nice - but recently I have had unreliable internet and I had some upload failures that I wasn't aware of until I saw a broken link for an image on the site. The current scripts are fast and easy but not at all robust for failures. Originally part of the attraction of the scripts is that there was at least a chance that method could be adapted for other storage services which I really like - but after having to diagnose some failures I am now seeing the benefit of leaning the other way and more directly supporting S3. This is long less flexible but it maximizes my benefit and programming time...
 - Minimum Amazon S3 upload window is now working - lots to do on this over time but the basics have already proved useful
 - A basic comparison between the local Generated Site and version on S3 is now working - far from lightning fast but faster than expected. Missing Files, mismatched size files and S3 files no longer in the local site are found. This seems pragmatic and reasonable even though esp. with the shorter files I can envision small changes that wouldn't generate a file size change.

11/3/2020

Working on the UI Context Tests made me think about the possible mismatch between what you type into the Tags and how the program parses it. The tag parsing was heavily informed by the need to process in the tags from Imported photo metadata safely which I think it does nicely but a test with ';' in the tag string made me realize that in the UI the parsing was overly aggressive and possibly misleading. Changed the cleanup for the user input to tags in the UI so that nicely allowing free form input and clean up are better balanced.

I writing test for the Created and Updated UI context found that with new items the load method was exiting before the control hooked into the child controls Validation and HasChanges - fixed.

11/1/2020

Added a search in the Photo List to pull up photos from the same date - this came up when looking for older photos where expanding to 'day' was an obvious/useful action after filtering for one of the photos.

Change the default css to have the same styling for tags and photo details.

Fixed missing help text on Tags and Folders.

10/23/2020

Doing the majority of sync via written file logs vs sync made me think again about how aggressive the code was about regenerating pictures - did a pass thru the code to reduce some of that by doing a better job detecting image changes in the editor and reduced some writes.

Fixed some small issues in the Changed HTML generation - the Tag generation remains on the todo but I have more confidence that other parts of the process are reasonable now. The most interesting detail was that atm I don't have any code to look at and only generate 'changes' to the Main Feed (Main Feed entries have a before) so to get the main feed regenerated I added it into the changes but that was triggering other content generations - pulled it out into a method that does just the Main Feed content if not in the changed list.

10/21/2020

I think the current round of improvements take the written file log to an obvious stopping point for now - small improvements, the scripts can now generate automatically to a file and explorer pops open with the file selected, when files are written is tracked and presented as a choice. I believe after a little work using this that this gets the core functionality taken care of. But this is also probably a classic 80% scenario:
 - This is an awesome feature if the user 'just' installs and configures the aws cli and permissions to run the scripts and everything goes right with the uploads (80%)
 - But a good implementation of this feature wouldn't require external program install/configs, would offer good error messages and a way to try again/recover and offer good progress information - getting to this is the last 20% but to build a custom Amazon S3 uploader will probably take as more time than building out the 80% did...
 So for now added Amazon Uploader to the idea list...

10/20/2020

Getting https://pointlesswaymarks.com/Posts/2020/cocopa-and-yuma-points-grand-canyon-9-30-10-1-2020/cocopa-and-yuma-points-grand-canyon-9-30-10-1-2020.html helped generated some small GUI improvements and bug fixes!

The most interesting part of this update was working with the written files log and the aws cli - this worked much much more nicely than expected so immediately generated some todos...

10/18/2020

Very Basic Gui to see written files - leaving this very basic until I explore what software/command line options there might be for uploading a list of files and what information would be needed (seems like they there will need to be some help generating the destination file/url).

Did a quick experiment with SourceGenerators for INotifyPropertyChanged - incredibly attractive technology but after trying and almost getting this working I think that it is better to wait until someone is willing/able to put this into an easy to reference Nuget package.

10/17/2020

I had decided previously not to integrate AmazonS3 functionality directly into the app - still reasonable, BeyondCompare has been easy to use but it occurred to me that it would be simple to keep a log of files that are written to potentially use as a list of files to sync. Created Methods in the FileManagement class that do both the file operation and write to the log table.

10/16/2020

Working on related content:
 - Found a bug in BracketCodeContentIds that meant all related content wasn't being found, corrected and simple test added
 - Related content was including Photo Content Id + Same Main Image Id pairs - corrected
 - Related content wasn't covered correctly in the first Changed generation code because without a record of previous related content there is no way to catch removals - data structure changed and routines updated

10/15/2020

Started adding in Line and GeoJson Content Types. Like Points these are more speculative than the other content types because they haven't been tested by long use/need on other platforms... Decided to keep line separate from GeoJson because line is likely to get dedicated ui elements for distance and elevation calculations and graphs.

10/13/2020

I want the generated sites to have high durability - leaflet is obviously a compromise, but I don't think writing a full mapping control is a reasonable and at the moment I don't quite have an alternative (I have been thinking about generating static images for the maps but I'm not sure...) - to help just a little with being durable I moved the libraries local instead of linking to a service.

10/12/2020

Changing approaches a little on Excluded Tags - I like the idea that I am not providing inappropriate pages where you can see all the images of a person (other potential uses but this is core), but as a result I can't see on the site who is in some pictures (annoying) and can't share with someone pictures of them in a reasonable way (not ideal). So trying a different compromise - Excluded Tags will appear as tags but with no link and will have a tag page generated but it will not be linked to anything - so if you know the url you can get to it, you are still 'named' in the photo, but there is no one-click solution to seeing everything you are in... Not sure - this thinking evolved from actually using the site so I think it will be most interesting to use it...

Spent quite a bit of time tracking down a newly failing test where I was doing >= to find newly generated content but should have been doing > - the could still go wrong in a chase where something else generated 'immediately' but I don't immediately know how that is a real scenario for now.

10/11/2020

Finished adding all my non-Santa Catalina 'outside' photos to PointlessWaymarks without any major problems. now that there are several thousand pictures on the site the biggest problem I noticed was the speed to check and generate tags - for performance it may be necessary to track changes in the database? Added todo.

Added a new folder and new cleanup routines for the temp HTML files.

ItemsControl wrapped in a scroll viewer is a great solution for listbox like functionality when there won't be any sense of selection. One interesting detail is how to handle scrolling for new items - for now I put together a simple behavior to scroll to the end - this of course quickly falls apart with more sophisticated additions.

Reworked the List Scroll on new item from attached property based to a behavior based solution and simplified the code. This code may not be as careful and safe as the previous code but I think this does the job in this scenario.

Found in the GUI editor for tags that excluded tags were not shown - that was not the intent since in the main GUI you should (obviously...) see all the tags. Fixed.

In some older photos found that in the details 'no' ISO wasn't handled correctly and the code thought the ISO appended to the beginning meant there was a valid value.

10/6/2020

Fixing small bugs inspired the start of a new test series on the GUI Contexts - hopefully I can steadily add to this for awhile. While it will be nice to have better testing I'm also glad to cycle back to actually using the software (working on PointlessWaymarks at the moment), it is a real advantage to being a 'real' user of your product.

Found that the WiebView2 NavigateToString method apparently has a limit on the size of the string (the error message is cryptic but reducing string size or writing the full string to a file and loading both fix the issue) which was causing problems in the Post preview - quickly switched this over to writing a file and using file:// but need to go back and clean up the files now. (Writing files had partly not been an option in the original WebView because UWP security restrictions made file access more complicated for the control).

10/5/2020

Sqlite's sqldiff.exe is a pretty great tool but one interesting detail for me has been discovering that when it diffs schemas it considers column order important - this made diffing some older and newer schemas rather tough with this tool (never found and an option to ignore order, never found a clever way to automatically order the columns in an existing db...) - this also is interesting from an EF Code perspective as order of the property names in a class do seem to matter when a db is created. I don't think there is a todo here but I cleaned up a few of the Model classes where there were fields out of order to try to help things stay consistent at least with newly created dbs. (As a side note I will add that the Sqlite Db Browser makes GUI reordering of columns quite easy).

9/21/2020

Worked has centered around Points - current status:
 - Gui Editor is working, of course could use more testing and use but is much more convincing now
 - Excel Export for Points is improved, after some false starts came up with a scheme for the points that seems to work
 - First rough test of importing a new point from Excel is done - this required a number of changes because the initial idea for the import from Excel was only use for updating.
 - Some initial Point Tests are in place - to make them faster to Work on started a new test series

9/15/2020

Icon set links - not currently using any of these (current source http://materialdesignicons.com/) but it seemed useful to note these here - past experience is that good open useful icon sets are not always so easy to quickly find via a google search...
https://icons.mono.company/
https://remixicon.com/
https://feathericons.com/

9/14/2020

Removed the Microsoft.Windows.SDK.NET and CsWinRT packages and updated the TargetFramework to net5.0-windows10.0.17763.0 and indeed did have access to the Windows APIs! I am a bit sad that this ties the program to Win10 for now (if I understand this correctly - I thought some of the Project Reunion/WinUI changes were supposed to reach back farther? Maybe in a different way...), one rational for WPF over some other technologies was the breadth of support across Windows versions - but at this point willing to sacrifice that to move forward with 'latest'.

The Text to Speech in the previous version was only working with debugging - went ahead and brought over more code from the Montemagno TextToSpeech UWP implementation - working now.

For the Text to Speech wrote a new behavior that get the selected text from the Web View so it can be pushed into a bound property in the UI. Good experience for doing more with sending data/messages to/from the WebView2. While I am not fully sure this should be in the ViewModel at all (is this just a UI concern?) this plus the TextToSpeech class made setting up a command in the VM easy - probably a good for now compromise esp. with the clean up this created for the BodyContentEditor.

Committed on a branch for another attempt at moving to .NET Core 5 - smoother this time both because 5 is farther along and based on previous experience:
 - With WebView you seem to be forced to upgrade to WebView2 because of the breaking change accessing Windows APIs where winmd references are no longer allowed
 - For this commit referenced  based on some GitHub discussion but ended up with a missing method exception and found another GitHub issue suggesting also referencing Microsoft.Windows.CsWinRT (to get the latest CsWinRT). Added a commit about possibly getting the same impact via a new TFM in the latest preview. I think the hardest detail for the moment might be getting up to date information, I didn't even come across the TFM information until searching for a later issue...
 - The WebView2 seems quite nice but it took some work to adjust to the new API - the current commit has some scratch test to make sure I had various details working - it will be interesting reworking the leaflet code.
 - Had a puzzler around SpeechSynthesis and playing the sound - after some false starts fixing this I found BackgroundMediaPlayer.Current to get a MediaPlayer fixed the issue without hassle

Before the 5 switch changed controls to monitor child controls for change and validation changes thru events.

9/7/2020

Finished out async Factory Method creation for editor windows.

Added a horizontal layout version of the Body Content Editor for the Photo and Image controls - didn't rename the original at this point because I thought it would be nice to find a way to do one control with alternate layout esp. with the speech and link code in the code behind.

Refactor into creation via async Factory methods for all not top level controls - for top level controls only the Photo Editor is currently refactored in part because at this point I ran into problems with the automated tests where TinyMessageBus was having problems getting a lock on the file it uses. I didn't debug into the TinyMessageBus code but after a little reading decided to change approaches slightly and push published changes onto a one channel work queue to reduce the number of possible places where TinyMessageBus needs to completely lock the file for writing (I believe that the combination I had in the code of new channel per publish and many publishes at once combined with all the reads was not working out at least across all the different threads...) - tests ran without problems several times after this change.

9/5/2020

Work on Point Details - still not completely convinced of the design but it is certainly a lot closer.

Had an interesting error today when loading Photo Editors - had an editor throw an error loading MetaData because the TitleSummarySlugFolder control had not finished it's background initialization. I think there are two competing problems - the constructor isn't async and whenever possible I do want to make sure work is offloaded to the background. Put a factory method into the TitleSummarySlugFolder to think about as a next step - removing async from the load is not a good solution I think...

9/2/2020

Continued work on the control refactorings.

9/1/2020

Continued work - this AM focused around the Title Summary Slug Folder control - on moving towards more use of the 'Data Entry' controls:
 - Added a Bool Control
 - Added a conversion based control
 - Added Validation to the Control
 - Added a multi line version of the string control
 - Worked on subbing in the controls esp. for Photos and Title Summary Slug

There is of course the additional over head - both in code and in understanding - of the new controls and esp. there setup, but it is quite pleasant subbing these controls in for the more verbose constructions esp. since with the Title Hover Help Text the new controls already offer more consistency. With the 'conversion' based controls what I like is that these are quite flexible in what the user can enter, but of course they don't provide the same level of help and protection in only entering valid values - with so few of these controls so far I like this approach for now and can always create more specialized options later.

8/31/2020

Created a very simple IHasChanges interface, added it to a number of controls and created a reflection based method to use in the Editor contexts to calculate Has Changes - the motivation is less reducing code and typing and more making it slightly harder to make a mistake by not including an added control in the check (ie while you still have to implement the interface and know about the magic reflection at least you don't have to manually add it to a list of controls to check!).

As part of the check IHasChanges effort added a utility property to handle simple string field change tracking.

Added change tracking to the Image Alt Field.

8/30/2020

The first implementation of the Folders ComboBox looked at the file system for existing folder but should have looked in the db... Changed over to querying the db and integrated DataNotifications so if you save a new folder in an open editor it becomes a choice in other open editors.

Start work on the Point Details.

8/29/2020

Changed the Elevation field to double? - really do need 'unknown' for this value. Added quick progress messages to the elevation service, corrected change detection in the Point Editor.

8/28/2020

Elevation work today for Points. In early versions of this program I used the Google Maps Elevation API for elevations - in the past I had found it gave very good results and it is fairly easy to use, but recently when reviewing some information about Google Maps and Leaflet I was clued in to rather restrictive API usage restrictions and, combined with the potential cost of using the API for the key holder, it no longer seemed worth including - both the elevation service calls and setting have been removed. I first tried https://open-elevation.com/ but got timeouts from the service and then found the rather awesome https://github.com/Jorl17/open-elevation and https://www.opentopodata.org/ - this not only offers a (limited) public API but also enough information to host your own version of the service (and, importantly, includes information on how to get the elevation data to power the service!). Because my usage is CONUS focused I currently try the NED data first and then fall back to the Mapzen data.

Elevation is now rounded to no decimal places. 

8/27/2020

Intended to use the Toolkit MapControl to take advantage of the UWP MapControl which I thought might dovetail nicely into the .NET 5 options and possibilities. But after some quick work starting to get it setup I had a mystery error when the control initialized that I couldn't immediately Google or fix. Since I am currently thinking about Leaflet on the front end I quickly changed plans to try to use Leaflet since I already know the WebView works. I setup a method to generate the html and javascript for the 'control' and push it to the page as a string - this plus some scriptnotify work and I was able to put together what seems like it will be a good solution - already map layers are selectable and dragging or double clicking sets the marker position.

8/25/2020

Work on the Point Control adding the editor, improving validations, more in the html generation.

In points refactored has changes to use the controls and to the control added HasChanges - the controls are tracking changes anyway so this seems appropriate.

8/24/2020

Made a quick attempt to use the AvalonEdit control - this still might be a great option but had an unexpected problem getting started because the control has no bindable Text property. This is for a good reason which is that the most obvious implementation of that would be a performance problem with large files and that is a reasonable target for the AvalonEdit control (apparently the WPF control uses a deferred rendering of 'Text' so that the property is not rebuilt on every change by default). The first place I wanted to try this was the Tags Editor - but after having some problems I also started to doubt that 'intellisense' style completion is really the best solution for hundreds of tags - I suspect I need to just rethink this UI piece a bit (atm leaning towards open textbox like now plus a filtered list where you can find tags).

Folder is converted to an editable combobox - simple and basically works but added some todos as in retrospect my first quick version doesn't get the correct data (I took the directories for the folders listing but should instead have taken the folders from the db for that content type - also should make it respond to edit changes in the choice list...)

8/20/2020

Fix a bug in the image list where a behavior was attached to the wrong control.

Created a Behavior to launch folders and links on TextBox double click - this is not perfect because it doesn't include any UI hints, but in other projects I found that for the time it takes to create something like this (very quick!) it is useful for power users since this quickly turns something like a read only textbox with a path into something you can click and see the folder - useful functionality and doesn't get in the way of building better UI later. This will take a little 'living with' to see if any exceptions are needed. I searching for ideas and solutions before doing this I was reminded of  https://github.com/icsharpcode/AvalonEdit and found https://github.com/Softwire/HighlightingTextBox/blob/master/HighlightingTextBox/HighlightingTextBox.cs - both added to the ideas list.

Added a method to delete unused Tag Html on cleanup.

8/19/2020

Fixes to the Migrations to get the older dbs up to the current version with Generation Tables.

I recently changed the number of lines scrolled via 'scroll wheel' on my laptop trackpoint - overall this was just a small experiment in getting slightly faster scrolling mainly in the web browser, but it had an unfortunate impact on the the scrolling in listboxes of this app causing the scroll to 'jump' much farther than makes sense (ie there were jumps where the new position didn't have a clear visual relation to the old position) - to deal with this I created a new behavior that ignores the size of the delta in a mousewheel scroll and just calls line up/line down on the scroll viewer - it is uncomfortable to change what is essentially user setting regarding scrollwheel behavior, but with the 'item' scroll of the listboxes I think this is a better experience.

Did some work trying to move the project to .NET 5 - basically I was stopped with available time because of changes to accessing the Windows namespace:
 - WebView - the older WebView is not compatible with the latest 5 preview because it includes Microsoft.Windows.SDK.Contracts which, at least at present, will break the build ->
 - Microsoft.WebView.WebView2 - it was unexpected to find the WPF support in this package - but the real unexpected detail is that it appears the wpf support is only in the preview versions even though the preview might not be 'latest'!!! So currently you need to install the latest preview even if there is a version equivalent stable. This did cause some API changes that didn't seem too problematic but didn't get to test this.
 - The change that finally caused me to give up is access to the Windows namespace - probably a good start is looking at https://github.com/microsoft/CsWinRT - but I doubt this is actually 'what I am looking for' and that something else will solve this issue as .NET 5 stabilizes - maybe https://github.com/microsoft/ProjectReunion is a useful reference here, or maybe there is a simple pre-generated drop in replacement for Microsoft.Windows.SDK.Contracts at some point?

8/18/2020

Basic Post Content Save Test added and a following test to test the change only generation - after a number of changes and bug fixes this is working. This certainly doesn't create anything resembling majority test coverage and validity of files but still nice to have (and an unexpected benefit is how nice it is to be able to generate a 'throw away' test site).

Added validation to the Photo Content control for file validations.

Pulled the validation and saving of Excluded Tags into a generator class - I anticipate test(s) with Excluded Tags and this allows saving in a way that is accessible in more places and can be tested.

8/17/2020

Ran into an interesting detail - I had been using ToString("O") to format the ContentVersion and GenerationVersion in the html data attributes - in testing the ContentVersion worked fine and the Generation failed... The issue is that the T4 templates for ContentVersion are fed by a DbEntry and EFCore + SQLite DateTime type always returns a kind unspecified, the generation version is fed from a code passed in DateTime so it had a Kind of UTC and the "O" format appended a Z to indicate this. While this didn't take too much thought the interesting detail for me was that this makes sense in terms of a SQL Server DateTime but in SQLite as far as I am aware EF core is storing the DateTime as a string and doing conversion back and forth, so not preserving the Z/Kind makes sense for consistency but doesn't make so much sense given that the backing field already seems to support any full fidelity DateTime data you want because it is just a persisted string? In any case I switched to using the O format but without the K at the end.

Fixed bugs in test and in the T4 templates to get tests passing again after the latest changes. In the T4 templates had an error where the generated T4 code calls ToString in such a way that passing it a null throws and error.

8/15/2020

The last few commits form an untested rough draft of improvements to the 'Changed' Generation - notes:
 - Added Tables and refactored existing tables to support Tag and Daily Photo change detection and to get a more coherent set of tables
 - Added ContentVersion columns to the Menu and Tag Exclusion Tables - this value is a core way of detecting changes and with Changed generation becoming more important it seems likely this becomes a standard field for ?all? tables going forward
 - Methods to detect and generate only changed daily photo pages
 - Methods to detect and generate only changed tag pages
 - Cleanup routine for the generation logs

Feature is largely untested.

8/13/2020

Spent the past few days working on importing all photos for HikeLemmon - the current total is about 4000 photos dating back to the early 2000s. This resulted in a few small changes but more importantly it seems obvious now that tags and daily photos have to be able to have reasonable change detection for html generation now that the sites are out to 4,000 photos over 2 decades so moving forward with work on that.

8/9/2020

Reformatted the xaml with the https://github.com/Xavalon/XamlStyler - in the past I have used the VS plugin but for a period I had problems with a lock/crash on save all so have moved away from that - but I miss the formatting work it does and when I saw the console/tool version I thought that was something I could easily integrate. No integration yet but did a first run and it was quick and easy to reformat all the files in the solution.

Worked on importing 2015 photos into HikeLemmon - as a result worked on File Renaming to be able to fix a problem where the imported photo has illegal (for this program) characters in the name. The fix is simple enough - rename the file on the file system and select it again - but much nicer now to just get help in the program.

One interesting detail was deciding how to handle the rename considering two concerns - exceptions/crashes and having the user not save the change. To handle both I decided to copy rather than move the file - this leaves a file to clean up later but nicely handles many situations.

Added the change info icon to Selected File and did some general clean up on the file presentation of Images, Photos and Files.

8/5/2020

Added cancellation support in images and files.

Added basic cancellation support for blocking tasks - only supporting method for the moment is the Photo import (the only spot at the moment where I am doing more than a handful of files at once).

In processing in 100s of 2018 pictures for HikeLemmon I ran into a bug in the data notification processing code where multiple notifications where running at the same time creating unexpected duplicates in the list - at first I tried making the processing code more error tolerant and having it eliminate duplicates, but this quickly led to conflicts with the collection changing... The code already handles notifications coming in unordered - but it can not handle multiple notification updates running at the same time - used the BlockingCollection based queue from https://michaelscodingspot.com/c-job-queues/ to receive the notifications concurrently however needed but process one at a time.

Some of the HikeLemmon pictures have an end date code in the title - I am still not completely sure if I want to import all of these by default, but will absolutely want some of them imported for specific use esp. in point data so added photo import code for that case.

8/4/2020

Photo Editor gets a close after save option - putting this in place helped me find a bug in the Link List implementation of this where I reloaded the data last in the method - which is efficient and makes sense but if you get a dialog or error on saving and the save doesn't finish you don't know if you have the current data loaded...

Tag List wasn't sorting - had a typo in the sort by field.

Adding re-filtering to lists after data notifications.

The lists use read only textboxes to present data so that you can select and copy the text - this is accomplished via a behavior that passes the click to the listbox so selection gets handled. This has worked surprisingly well for such a quick and simple solution but it has meant that if an item is selected by a passed click that subsequent use of the arrow/page/navigation keys won't work - because this behavior is only attached to read only textboxes I took the same approach as the mouse click and pass the event along after marking it handled to avoid any doubling of the key press. This is working great in the first tests and it will be interesting to see if it holds up.

Started working on Points.

8/3/2020

Data structures for the PointContent - point content was in the initial version of the project and I did some work on Points/Lines/Segments but in the end those turned out to be secondary to getting the current content types working, but now circling back around to this with, I think, a slightly better idea about how to structure the data.

In looking at the databases to add the new Point Tables I realized that I had missed adding some indexes - the dbs are small so for performance it doesn't really matter but many of the indexes have unique constraints which are the ultimate protection against duplicated ContentIds.

Control C for the link list for url to clipboard implemented (this is essentially the link list version of email html to clipboard).

Main window Tabs now load only if selected - not perfect (it should setup everything in the background while you work!) but a very practical quick fix to the interaction delays I was experiencing. I think the hard detail here is that I could design more of the data to load without UI thread interaction, but the challenge is that with a larger complex list it seems like there is a significant hit on initial load (and in fact maybe enough of a hit that the difference between pulling db data and GUI load and already pulled db data and GUI load is not enough to matter)...

8/2/2020

Added Html Email to Clipboard functionality to most content including some refactoring to support the process - still some todos from this but I believe the basic functionality is done. Not sure this feature would connect with other people - almost by definition everything created here could be sent as a link to a webpage with better formating and better support (HTML email/email clients are just not as good as browsers...), but this feature is from my own experience where people ask you a question and they want an answer, not a way to get to an answer and pasting in content and adding a quick reply could be really useful.

8/1/2020

The recent San Pedro email showed some problems in the email template so reworked it into a form that does work with that email as a test. This translation puts everything into rows, centers pictures using the 3 cell technique and varies the width setting based on horizontal vs vertical size.

Noticed that a snapped right window didn't reappear in the same spot on startup - adjusted the EnsureWindowIsVisible to not pad the height when checking (the original padding was probably for the task bar but I believe dpi scaling has made that value, even the 'default' more varied) and more tolerant about right side off screen (since while the math seems correct the app snapped right is supposedly off screen but is not? something seems incorrect but with a good fix in place not digging in more to screens and scaling at this point...)

7/30/2020

Quick hack in of local no cost text to speech! In the Body Content Editor to help with proofreading.

Added Json import and export for the Menu Links and Tag Exclusions so that the Json information on the site is complete - I am still convinced the Json Files from a nice part of a layered backup strategy - looking at the Json again and thinking about backup made me realize that if the settings file was split into public and private that the public part could be tracked in the db and copied to the site which would facilitate triggering generation based on changes to the settings and backing up the settings.

Worked on LinkUrl validation and validation indicator.

Added Excel export for the diagnostics and changed the Excel naming convention (with the files all saved into the temp directory the date is now first - I think this is a no win where depending on your needs this will quickly be wrong but I think this is a better default - worth noting too that since the files open automatically there is a good chance for the user to save the file where and how they want).

Added a clean up routine for the Log db to only keep a couple of months worth of logs.

Changed the temp file clean up to 28 days - there is a chance that a user could have files unexpectedly deleted which I don't like, but unsure whether letting files build up forever is ok? Maybe now that the files are default date sorted?

Changed the Main Form commands to the newer style.

7/29/2020

Refactored the Command Syntax. Commands in the project were verbose in large part because my preference is to call what runs in the command thru one of the 'Run' methods in the StatusControlContext - in other projects I have reduced the verbosity by creating Command Classes that encapsulated the call to the StatusControlContext (so for example a 'BackgroundTaskCommand'), this has worked reasonably well but here I took another approach that eliminates having to create the custom command classes and gave the StatusContext classes that create and return a command to use. Light testing appears this is working and the code is more expressive and pleasant! (Note: I like the approach of commands calling a wrapper in the StatusControlContext because it provides quite a lot of functionality and utility (both UI cues (the spinner for non-blocking tasks, a full screen progress list for blocking tasks), backgrounding Actions/Tasks and catch/display exceptions) for very low effort. I do think this pattern is a 'shortcut' - I don't think anyone would have to try to hard to find objections and alternative approaches especially in an 'ideal' or 'recommended architecture' situation and if I was writing an enterprise app with a team I doubt I would use this approach - but for this app it seems very attractive.

Added some basic formatting to Excel - by setting some limits on the column width and row height after autofit runs I think the format is much improved without the need to create a plethora of specific formats.

7/28/2020

When I added the model for LinkStreams I didn't want to call it 'Content' because the links don't get a 'content page' they are always intended as just a list - at the time this difference seemed huge and important - over time that concern faded as everything is in a list and as more operations work over all 'content types' and it became somewhat mentally off putting to have this naming exception.

First version of the DataNotification support in the tags list - light manual testing suggests this is working nicely!

In looking at the DataNotification support for the tags list I made a bug fix - errors were falling thru rather than returning - and improvement - data notification subscriptions detach before load and attach after load to minimize the overlap of working on the data via notifications and loading overlapping - to the list data notifications.

7/27/2020

Realized that the 'Changed' only generation was missing detection for content that has been deleted when determining whether to regenerate photo gallery and list content. This triggered several changes:
 - Added methods to the DB class for deleting content from the DB and refactored the lists to use this - previously the delete routines were in the list contexts, this makes it easier to reuse the methods (for testing at this point)
 - The list delete refactoring enables multi item delete in all lists, improves the multi-item delete message and moves the responsibility of removing items from the list from the delete method to the data notifications
 - Added DB Methods to get the last historic entry from any 'completely deleted' items, ie historic items where the content id no longer exists in the content table, and used these methods to help determine what lists and galleries to generate in the Changed Html methods.
 - Added a basic photo test for delete, get deleted and re-save.

7/25/2020

While working on the Image Testing ran into a situation where it appears between the .net, sqlite and json datetime representations that, at least as I was handling everything, some precision was lost in the ContentVersion. ContentVersion is now truncated to seconds on the DB Save - the precision simply isn't needed and between all formats it seems as if seconds is handled without any additional work which is a huge bonus.

7/23/2020

Fixed a bug in the Excel Import where Content Version was not being set causing the Change Detecting import to not see all the changes - fixed by moving the Content Version assignment into the db save routines and removing other places they are assigned.

Fixed the Status Control on the Tag page that wasn't spanning all rows. Improved the changed message for the Excel Import - the change to the friendly printing reporting was nice but a newline was missing between the title and changes.

The Tag List UI is now in a more reasonable place. For a general purpose application the current design is easily criticized as not giving intermediate users good options - for beginners the easy option to edit each content item for a tag is probably best - and for advanced users it is hard beat spreadsheet style editing (Lightroom might give an interesting model here with the way it *s tags in multiple photos - but I don't think that is actually as good as editing in Excel even though it is very workable for advanced users in a compact space that doesn't require moving away and back into lightroom... And worth noting in Lightroom for confusing/messy/problem edits I think many people at some point end up wanting to edit and reimport from a spreadsheet...). But best for intermediate would be some editing tools to rename/delete/merge tags in the UI - leaving out because of a combination of the complexity of doing this well and the extremely high utility of the Excel import (fwiw I wouldn't rate this as the best 'beginner' interface, I think there is just too much possible confusion of what is changing where and the consequences of details like merging or splitting tags even with a really beautiful interface that guides/warns, provides undo/redo, gives a great display of what is changing).

Take advantage of the 'changed' information and add a few more filters to generating only changes - Daily Photos (which were originally designed with 'interlocking' previous and next pages for generation) and Tags (where detecting changes is not simple) are still 'full' generations for now.

7/22/2020

Overall 'Generate All' time is not currently a concern - it is in set the computer down and get a coffee range which is essentially on target for my goals for this project. What is getting tougher with  2,000+ photos is the sync to Amazon S3. One issue might be that I am currently using BeyondCompare - it may not be the ideal tool because of the slow scan of S3 for changes and what seems to be single thread upload to Amazon - but regardless of tool choice (and short of custom tooling) with any tool one item that is currently becoming and issue is that the All generation touches every html and JSON file adding up an impressive list of changes on items that likely didn't change causing way more delay updating the site on S3 and dwarfing the time to generate.

To address that this update detects changed content and related content to give a 'generate changes' option. This could be taken farther as the Daily Photos, Lists, Tags, etc. still always regenerate but I think this is a good starting spot.
 - Routines to extract bracket codes and main image and write a related content table - intent here is this is wiped and regenerated every use
 - Started writing the last generation time into settings
 - Routines to detect changed and related content and a db table to hold that info to easily join content against in getting content to update

 Related to this the Main window had quite a bit of key HTML generation code in it - refactored that back into the data class and did a light rework of the related tabs.

7/21/2020

Changed the Excel import differences string to a format suggested by https://github.com/GregFinzer/Compare-Net-Objects/wiki/User-Friendly-Report.

Added methods to purge content not in the db from both generated content folders and the Media Archives - made these accessible in the Main form but at this point did not integrate them into the 'generate all' process, still wondering if that is the way to go...

Updated the data notifications response codes in the list context to the better form found in the Photo list - the idea here is to basically distrust current state and message order and regardless of whether the send says update or new check and add it only if not already in the list and update in either case.

7/20/2020

It was great to get the Excel Import integration test working early in the process but today's changes are a result of that hitting me doing 'actual real world' importing and it produced a lot of changes.
 - The Excel import wasn't validating content until trying to save - now it validates it before returning info on the import.
 - The import last updated by is now excluded from the object comparison to avoid falsely registering a difference when the only change is the updated by - this could be more sophisticated and does leave a hole where if you were really trying to update only the last updated that it would be frustrating but I think this is good enough for now.
 - Improved the Excel import progress - still chaotic for sure but better than showing nothing
 - Imports with files were set to regenerate the files - changed, that doesn't make sense since the selected file is not going to update in the import
 - In working with an import from excel the high precision times that EF was storing were triggering changes as ClosedXML was picking up only readable times (to the second) from the default format. In general I don't like loosing time precision but in thinking about 'by-hand' editing the full precision is not helpful. Added a DateTime helper that reflects over an object and truncates DateTimes to seconds - I guess this could play havoc with the apparent ordering of high speed photos but that is a highly imaginary concern at this point.
 - The Excel imports were partly inspired by some Photo clean up that I needed to do - in working on that I found tags that didn't meet the current tagging limitations on characters - ultimately I decided to make the tag clean up more aggressive. This could create some frustration with not being able to fully enter what you want but should help keep the tags clean in the way I currently am thinking about them.

Ran into an interesting issue with the Status Context dialog - I was using Task.Delay to block for 3 minutes so that the program didn't get 'completely stuck' in a dialog - I think that concern resonated most strongly when I was first working on and having trouble with setting everything up correctly on the dialogs... I finally let a dialog 'expire' and an import started that I wasn't actually ready to run yet. After some searching added a WhenCancelled extension method for the cancellation token to make it easy to wait as long as needed.

Started the first bit of rework on the tag list - changed it into a true listbox and added Excel Export (added a supporting class the implements ICommonContent as an easy way of getting most fields for most content types all together in one list) so you can work in Excel and import tag changes to multiple content types at once.

7/19/2020

Excel Photo Update import test is passing.

Validation improvements and fixes including a model Interface simplification.

Added to the Generator Save methods a routine that uses reflection to run null safe trim to empty string on any incoming string fields - that plus a tag cleanup should produce a cleaner more standard db input.

Added a pinned dispatcher property to ThreadSwitcher to facilitate setting up and using a consistent dispatcher in situations like tests where Application.Current.Dispatcher is not going to work.

Working photo context test! Haven't test a backing class like this before - still doesn't tell you if it is actually wired correctly to the UI (and as noted maybe testing the GUI is the way to go - ie if it isn't wired up and displaying correctly does it matter it is technically correct?) but very interesting to get this working.

Failing Excel Import/Export test added - not sure what is failing but happy to get this into testing!!!

7/18/2020

Excel Import - the first Excel import code was photo content specific - currently I have more photo content than anything wlse on PointlessWaymarks.com so that was the most benefit. But in working on the code it became obvious that it was going to be reasonable and possible to design one import to handle the current content types - so over a few commits put together code to do the imports. Only 'first ad hoc tested'.

7/15/2020

Consolidated some validation into CommonContent Validation.

Fixed a bug in saving to make sure that the content editors don't reload data if generation fails (which could both leave you in a bad state and potentially wipe out your work or error trying to load null).

The TitleSummarySlug and Tags editor now have validation error and warnings (only warning is in tags) for some validation (the slug is unique validation is not run at the moment to avoid issues with performance and/or more complicated issues of caching/delay bindings/etc). Working on this triggered a number of changes tightening up validations and some additional tests.

Removed ObservableRangeCollection - this has been great for me in XamarinForms but I ran into some issues in WPF, potentially fixable but as with every time I have tried extensions of ObeservableCollection in the past it seems easier just to deal with the threading than work around...

Removed the last of the older Event Notification code that is now switched to TinyIpc and at the same time move the Image Content Editor over to the ImageGeneration.

Rewrote the DataNotification code in Photos - basically I had realized in previous work that the update code I initially wrote assumed 100% in order no error transmission and event creation from the sender - which even for small single app messaging seems improbable to be always true so code is rewritten to honor delete vs update/create but in all cases to check first for the GUI contents and update as needed. I anticipate all the lists moving to this style but for now testing in Photos.

7/14/2020

Used https://github.com/cyotek/SimpleScreenshotCapture/ as the basis for modified code to capture window images via native methods so that you can capture both WPF content (as you would with any WPF technique for images of windows/controls) and XamlIsland type controls also (which are rendered differently and don't show up at least with the WPF techniques I have tried).

Finished out the conversion from FontAwesome to XAML/SVG Icons - more work than expected overall setting up the user control and switching out icons everywhere but I think this is a good trade (especially since so far the first version of the UserControl seems to be holding up for both icon and in button use) - of course it is not reasonable for this project to not take dependencies, but I think in the case of FontAwesome it is unreasonable to believe I could pin the current version and work for years without anything breaking, I do want this project to stay 'up to date' - but I also have ambitions for this being something that I might still use in 5-10 years, overall Windows Desktop software has proved remarkably durable and tooling from MS very supportive of older frameworks/projects (this is based on experience at work where things have run for many years between edits...) so I think this might be reasonable and but I don't think it is reasonable for FontAwesome which was a core-non-optional part of the GUI.

Fixed a bug in the Photo List where reloading wasn't properly applying sort and filter.

7/13/2020

More work on the newer test setup - this moves the test coverage backwards temporarily but I think the refactoring/restructuring to the tests will pay off - good first start on refactoring Photos.

While working on the image processing I accidentally found that the System.Drawing out of memory exception I had gotten on some images (which it is well known can be a format or other issue and the out of memory is not an accurate error) was potentially caused by a long file name bug. I originally thought it might be long metadata title - but I got a more informative error by eliminating this use of System.Drawing and using Photosauce Magicscaler instead. Magicscaler returned a Directory Not Found exception, but the fileinfo object generating the filename reports 'exists' and I could navigate to the directory and file. Some research turned up long filename related bugs - https://stackoverflow.com/questions/5188527/how-to-deal-with-files-with-a-name-longer-than-259-characters - and the extended path length prefix. Passing file names with that prefix to Magicscaler fixed the issue.

Replaced FontAwesome in the TitleSummarySlug editor - this turned out to be more difficult than I imagined with tooltip display and flexible sizing, to get that I ended up with a UserControl that seems overly complex for what I want to accomplish but does currently seem to meet the goals...

7/12/2020

Thinking some about writing more integration tests it occurred to me that it might be more useful to build something 'coherent' so that making it more complex made sense and so that the result would make more sense for visual inspection and perhaps as a decent testbed for css and new features. To that end spent time this AM getting a few Ironwood Forest National Monument pictures and PDFs setup to start with - this will mean time reworking some of the existing testing, but I think it will be worth it to have a way to build a 'more real' test site.

Added the html photo metadata report to the reports menu in lists after a small refactor on the report code in the photo editor - finding it quite useful to get the metadata into a format that you I can search without an additional step.

In the photo metadata extraction added Taken By and License fall backs to xmp and iptc.

7/9/2020

Reworked the organization on the data project - I'm not 100% confident that this matters or is 'right' but it does feel immediately more reasonable.
  - The T4 templates were the biggest problem - on the eap version of resharper I am currently using the ForTea plugin doesn't seem available and I wonder if there is any chance it could have helped? At least the generated content gives good errors to work from - but I wonder if it might be time to look again at Razor templates, last time I looked it seemed possible but also messy.

7/8/2020

Changed out Photo and Image types to have Body Content - my original thought was that a Photo with Body Content should be a post, that still strikes me as reasonable but recent images like the Roskruge map made me think about how some images are 'pure images' and some are 'resources' where the page for the item should present more information as needed. I had already started down this path a bit with the 'Image Source Notes' field for images, I think this just takes it out to a logical conclusion. At this point too based on the use of files I am more comfortable with 'views' of a file/image/photo that does give you the Body text and don't feel the need to incorporate it in every view...

Had a slightly unpleasant experience with FluentMigrator where Rename and Delete columns were silently not running in migrations - this is familiar at this point as Sqlite has traditionally not supported some operations that other DBs do and working around those limits for all situations is challenging (I think Sqlite recently added column rename support so maybe this changes in the future! There is some basic explanation of the issue here at the bottom about why https://www.sqlite.org/lang_altertable.html). Changed the migration over to a sql script - because I have kept the db setup simple the script to do the changes is verbose because of all the columns but conceptually very simple and fluent migrator does make it easy to run.

While working on the migrations found that I didn't have the correct matching columns in the HistoricImage type - fixed - I wonder if I should revisit how this code is written and explore using inheritance to ensure that the types are always in sync. In the past I have had trouble with inheritance and EF not acting always as expected but probably time to revisit that and see both what comes up now and what I can do with some of the ignore options.

Removed more old code.

Fixed bugs introduced in the FileGenerator work.

A little more StatusControl appearance and layout work.

7/7/2020

Basically while adding resources for https://pointlesswaymarks.com/Posts/2020/empirita-ranch-6-18-2020/empirita-ranch-6-18-2020.html I found that in some cases I want images to do some of the things that files do because they are serving the same purpose in some cases like the Roskruge 1893 Map of Pima County - resulted in three things:
 - Bracket codes to hyperlink to an image rather than showing the image - added for photos also
 - Added a command to open an Image from the Image List.
 - Added a todo to update images to be more like Files in terms of content setup

Small changes to the StatusControl - thinking about changes for better appearance and layout.

Bug fixes for Tags (wrong value) and Index (wrong method called for type).

7/6/2020

Added a File Generator and Integration tests.

Cleaned up a few compiler warnings.

Removed old code.

7/5/2020

This app is all about a single power user working on the desktop against a local Sqlite db - given that scenario two problems that come up:

Problem 1 - Cross App Data Change Notifications:
Because lists of things and editing those things is a very common pattern there quickly becomes a need to update the list as edits are saved in another window. Previously in this app I did that via a centralized class and Weak Events (thomaslevesque/WeakEvent: Generic weak event implementation - https://github.com/thomaslevesque/WeakEvent/ ) - MVVM Light Messenger is representative of an another  pattern that can also be used and either solves this problem rather nicely.

Problem 2 - Interprocess Data Change Notifications:
In some cases solving problem 1 is all an app will ever need, especially if you put a single instance constraint on your app. But I have found that if you have 'power users' that may launch multiple apps or have a scenario where multiple apps (or the app and a long running background 'script' app) work on the same data (and likely share common code to do that - say both an Inventory Management and Purchase Order app that both can show and edit Vendor Information) you quickly end up with a real scenario for Interprocess communication. Searching for Interprocess Communication for windows desktop apps devolves into many solutions that could work - recently I found a new library that I had not seen before that seems to work in initial experiments and takes care of an important part of dealing with this problem which is nicely wrapping up the native windows calls into something nice to work with in .NET - TinyIpc https://github.com/steamcore/TinyIpc - from the GitHub pages: ".NET inter process broadcast message bus. Intended for quick broadcast messaging in Windows desktop applications, it just works."

While at the moment Interprocess updates is not an urgent need I did go ahead and update to TinyIpc because I want this for myself and I am especially intrigued with the idea for me (or maybe super power users) that you could do things via a cli and have the updates available in a GUI running at the same time (without network, polling for changes or having a db that can broadcast notification).

7/2/2020

Changed the Photo editor and photo automated import over to use the new Photo Generation - resulted in a few changes in the Photo Generation to accommodate needs in the GUI code but was generally smooth. The interesting thing about doing this is that even though the base methods were extracted and are now tested (good!) there is still quite a few details to get everything correct in the GUI - I'm sure that the code could always be structured better to minimize concerns but I also suspect that long term some UI Automation testing could pay off - adding it to the todo.

Fixed a small layout error in the Main Window General Tab.

Started work on a routine to clean/purge the folder structure - the motivation here is that the older GUI Photo save routines detected and moved folders/files as possible but for simplicity the newer photo routines do not do this so another cleanup strategy is needed.

7/1/2020

More work on the Photo Import Testing - it could of course be more comprehensive (probably most missing is some validation of the html - or maybe checking specific elements?) but I think this is a great first 'safety' sanity check to get in place and should help the project to move in this direction for all content types.

Updated the data notifications in the Lists to repspond to the newer local content update notifications - also a small refactor to more easily get the gui image url without getting tripped up on nulls or other invalid values (of course truly invalid data is very important - but not here, these gui routines should display what is possible to display and as much as possible not crash due to a missing picture/invalid data).

In the photo testing for the JSON data files I wanted to compare all properties and found CompareNETObjects to be very useful - https://github.com/GregFinzer/Compare-Net-Objects!

6/30/2020

Have a working single working test of the Photo import with asserts for the imported metadata! The test could probably be setup in a more useful way but for now this seems like it is a great jumping off point to getting more tests around the Photo Generation and then use this style to test other generations.

6/29/2020

Continued work on the Photo Generator code that is basically a refactor of the GUI Photo Editor code - got to a point that seems complete enough to put in a test, but had a quick first failure on the test because it appears I am building the basic site structure but not setting up the db... Some details that came up working on the Photo Generator:
 - Added a method to delete image files in a Photo folder where the image is not related to the current original image - this is essentially more brute force than the previous GUI version but I think is the best because it doesn't require knowing before/after details rather just acts on 'current'
 - While working with the data notifications and thinking about issues in the past it occurred to me that it might be an interesting approach to notify both on db changes (so the latest text/entries/etc is displayed) but also on 'local content' changed, since some GUI elements use images/generated content for display separating these two might allow the GUI to keep up in a nice way.
 - Made a 'for now anyway' change to validate each file name so that URL escaping is not needed for content URLs - it might be better to instead make sure that URLs to this content is always appropriately escaped but for now I am interested in minimal hassle and as much parity between the local files and site files as possible.

6/28/2020

Jumping around a little to work on some tests - now have a start on creating a test site in a nunit OneTimeSetup, added a test that checks that the very top level folders were create and added two content files to the test project for future use. Working on the methods that check that media files are both in the media folder and the content folder made me think about how easy it would be to make a mistake that types/compiler would not catch - hoping this is a good start to getting some tests in place on all the file and directory creation.

6/27/2020

Expanded some on the GenerationReturn work - I like some of the details although I am slightly torn over whether this should be 'just' a log type, ie is it worth having log entries and the Generation Returns that also write to the log? Generation is such a special case I think this is 'worth it' but it is frankly a little hard to say.

6/24/2020

Did some work to start trying to refactor the site generation around what I know now after working with the system - the main two items this go were to start building methods with a Generation Return object that allow better information return considering that one of the needed flows is to log an error, keep going and then pass the error information along. I don't think this is in it's final form yet but I think the current object and the methods around confirming all the folder structure and base media files are setup are a start.

6/22/2020

When I first found FontAwesome for WPF I was really thrilled - but a recent library update broke this functionality, could have been my usage or bad luck or ... and I was able to role back to an earlier library without issue - but I think in retrospect that what I should do instead is to use icons like those from http://modernuiicons.com/ and use a control for the 'wait' spinner. This version starts the process with a spinner control from http://blog.trustmycode.net/?p=133 and https://github.com/ThomasSteinbinder/WPFAnimatedLoadingSpinner - I believe this will be more durable in the long term and especially with the extreme durability, at least in the past, of MS Desktop technologies there is a benefit to leaning in when reasonable to longer lasting approaches.

This is the first generation of the site that I ran into an issue where a page had a bracket code to a non-existent Content ID - I added some guard code, changed the generation ordering and added some code around checking for and generating missing content but this need more attention both potentially in 'pre-checks' and/or in making sure that in as many cases as possible generation can continue and a good error report is generated.

6/17/2020

The photo imports previously tried to process all items at the same time with a number of files under 5 - but today had an error related to a daily photos related content thumbnail and generation that I believe would have been prevented by the sequential style imports so switched over to only that.

More Tag display/edit work including some refactoring on the Save code.

Nuget Update.

6/15/2020

Changed the way the Photo Reports work - initially I set up a Expression so that a new db query could be issued with it - this approach is very clean for the 'no tags' report but in the Title Date and Photo Created Date mismatch report I needed to parse the Title to both check for and extract the year and month, certainly possible in pure sql but easier to just process all the entries in C# code - so the report function now generates the list rather than being an expression for filter and EF Query.

In working with the mismatched Title and Photo Created dates realized that when deleting I was writing a last historic entry (good) but leaving no record of the date of the deletion - changed the list deletes to have the deleted date as the last updated date, this is a slight bend of the way things worked before where historic was really only ever a pure copy but this records the data nicely and it isn't hard to argue deleting the entry is a type of update...

Fixed a missing command on the newer button to flip between load all and load recent in the Photos.

In the photo import added some logic to strip out multiple white spaces - this is slightly questionable but I felt like in bulk photo updates this was a slight advantage in getting normalized titles - but on the other hand if you had a common in your title and wanted two spaces after it that is pretty reasonable? Not quite sure here...

6/14/2020

No Tags report implemented - first use of the menu bar in the app - there may be enough actions and future planned actions with the reports that it might be worth it for organization now.

6/10/2020

Delete in photos was loading after the delete finished - this was an easy idea in the beginning but was essentially a bug after the addition of recent/all - changed.

Make the Photo Load Mode more sophisticated to setup for using this control for a photos report window - because Photos lean heavily on imports rather than entering all the info it is pretty easy for unintended items to slip in - a good example might be 'photos with no tags' - not necessarily an 'error' but at least in my usage unintended and worth reporting on, another example since I heavily use a date prefix that the program processes a mismatch between the taken date and the title date and missing license or copyright...

6/8/2020

First version of the Menu Editor is in place and the generation is now using this - nicely simple since the current menu implementation is just a flexbox of links - originally I thought about reworking this to something 'more' but this style has grown on me, still needs consideration for mobile but... Added basic help.

Fixed a bug in moving folders in posts where a new parent folder wasn't created when needed.

First use of the Special Page bracket codes in the Subscribe page - worked well. Updated the help to the bracket codes to reflect this.

6/6/2020

Setting up for a MenuLinks table so that the current 'core links' is database driven. This should be a nice step towards being able to quickly generate other sites.

The emerging most popular use for images is to show content in files esp. via the pdf image extraction - this is great/as intended but an aspect I didn't anticipate is that many of these images essentially create 'duplicates' in the search results that are not helpful and take away from finding the file/better content (the file and the image are likely to have the same thumbnail...) - added a ShowInSearch column to the Image table to be able to control this. Most interesting issue with adding this was that an image excluded from search could have a unique tag - so tag pages are generated for all tags but the tag search page only gets tags from pages included in search.

6/5/2020

While saving a Pima County 1999 Mountain Parks report I found I needed to rotate an image - normally with a photo this is corrected before it hits this program since the current main photo source is Lightroom - however in this case because this was an extracted image from a pdf and this program is actually the quickest way for me to do that it was obvious that rotation in the UI would be a benefit - thankfully an easy change by combing MagicScaler and the recent change to the way the editor image/photo is displayed.

Taking memory usage off the todo list - the UI changes plus the switch to MagicScaler seem to have at least improved the situation enough to let it go for now.

Via a little string manipulation to replace - with spaces - this could eventually need data structure changes but for now this works for the display of tags.

Bracket Codes extended to support 'Special Pages' like {{index}}, {{indexrss}}, {{searchpage}}, etc... In the process put some tests into place on the Bracket Code processing and improved that code! This is partly for general use and partly for the upcoming MenuLinks table where bracket codes will be valid and as a default setup using the bracket codes creates one more spot that where changes to this code or a site should be less likely to break links.

6/4/2020

Switched the content lists over to 'Item' caching and scrolling - while the scrolling is not as nice as before the performance impact on Loading the Photo list was absurd - where before it would pause for ?1 minute or more? to rebind the ObservableCollection with 1000 or so entries now it loads in <10 seconds (which for the functionality of an 'all' list without paging seems worth it to me for the moment).

Remove ImageSharp in favor of MagicScaler. With use of simple/minimal code MagicScaler is notably faster and I appreciate the switch to an Ms-Pl licensed library.

Add a more aggressive image clean up setting to better clear out old/erroneous images.

6/3/2020

Switched the main resizing over to MagicScaler.

Beginning to look at memory usage a little more -
 - ImageSharp appears to be holding quite a bit of memory in some cases although I am not sure what the expected GC pattern is here so I am not sure this is a 'problem'
 - I am still suspicious of the Photo Editor form based largely on watching memory (which I am completely aware is 'anecodotal' evidence at the best, I absolutely don't have internals knowledge to let me predict what the GC is going to decide...) - in looking at the code I did identify the opportunity to rescale the selected image in memory for delay on the form rather than referencing and downscaling the full sized image - there is some up front cost but this appears to be a win. As part of this read a number of posts around a Google search in the spirit of 'wpf bitmap memory leak' which are not encouraging and also many of the results are old and there is at least some chance WPF internals have changed.

6/2/2020

I had an out of memory error today after some heavy photo loading - a super quick check of the default Diagnostics view in VS suggests that I am not freeing something related to photos and memory usage goes crazy by the end of a large export - nice that the program has been staying responsive, but I need to track down the memory leak...

On HikeLemmon email subscription popularity was a surprise for me so I wanted to find a way to do it in here. So far:
 - Rather than a custom page I just did an iframe in a post page - this will never be custom beautiful, and another more beautiful approach might have been a custom page that abstracts the typical sign up process, but it seems highly functional and practical and I suspect means minimal maintenance and no additional credential storage or other abstractions.
 - Added an 'HTML Email to clipboard' function and supporting methods - right now anyway 'posts' are very simple so only does some minimal common email formatting to get a simple email together. One challenge is that my simple posts are 'by convention' and a Post can actually contain anything markup legal (so basically anything) and email HTML compatibility is very terrible... However this solves the actual current problem in an acceptable way.
 - Signed up for SendInBlue and connected google reCaptcha - their free plan seems to currently cover all current needs, first test went well and the API docs look encouraging (important since a 'publish' action rather than HTML to the clipboard is the eventual plan).

Caught a bug in the bracket code refactor of images - fixed.

5/31/2020

Added a table and migration to hold pairs of related content - this is not wired up/processed yet but the idea is that this table will track related content (which otherwise is largely trapped in the text of a post in bracket codes) and allow generation of a change list for more efficient html generation and perhaps a list of file changes.

To setup for tracking related content the Bracket Code was refactored - both to have an easy way to get the related content and also to pull out some common code.

Added a migration for the new related content table.

5/21/2020

Supplement the title with the file name if the title in both XMP and IPTC title are blank - originally I thought this was a mistake because it can leave you with Titles that are whatever file name format your camera is writing, but in retrospect I think better to try to find some valid data rather than leave this blank.

Created an MSIX and installed it on my computer. This was largely a test but since it was successful I will probably try to use it and may try to use Github actions to build it automatically.

Included the bracket code help in the post help.

5/13/2020

Small fixes to the Image created from the File pdf extraction.

Quick updates to add pure to the remaining reports.

Photo List is now limited to most recent initially - the list is still smooth and responsive with all content loaded so I don't yet see the need to have search and basically limit getting to show all - but the load on show all is painfully slow as an initial load when you 'requested the program start' not 'requested all photos load'.

Implemented a very simple history report in html for all content types - originally I assumed that I would implement this as a wpf window but the since currently the 'real' idea here is to show history and let you manually merge what you need (ie - whole body got erased, no problem - just copy and paste it back from history) the html seems like it gets more functionality more quickly.

Found a bug in the Photo processing for my date patterns at the start of names - fixed. I think this is the last change here - I do wonder if this pattern should be a setting, it is pretty personal and will now catch more patterns...

Extended deletes in the photo list to allow multiples - so far I haven't needed this in other types so leaving those as single delete only for now.

Bumped up max photo load number to 100.

Most recent file is moved to the top of the recent list.

5/9/2020

Finished out a first implementation of the year and month navigation in the camera roll - I believe the structure will work but will need to work thru the styling a bit more to know.

Finished a Help Block that lists software used building this software.

Added PureCSS to the project as a static copy to inline into the photo metadata reports (and/or future reports) for quick and simple styling - didn't link it because I want full offline functionality with these local html files.

Originally in the metadata reports I opened the photo metadata report in an HTML view window but either the state of WebView, or the state of my use of it, means that find and context menu are not coming up?!? Switched that report to write to the temp folder and to open thru process start.

Added Camera Roll to the parts generation.

Added an about tab to the main app.

Bug fixes in new XMP Lens data extraction.

5/8/2020

Had trouble with a large panorama 1910 Sea of Sand - about 33k wide - throwing an exception in the ImageSharp based resizing code. From the GitHub issues it seemed like this shouldn't happen but it was slightly difficult to follow the technical library/image processing discussion plus be sure I understood what was in the currently packages - so asked in the gitter channel and 'an overflow in JpegFrame in SamplesPerLine' was identified. For now I just used a 20k wide export for the file - but hope to change this out later.

When working on a new post I found a puzzling truncation of long Titles extracted from Metadata - I reported https://github.com/drewnoakes/metadata-extractor/issues/474 but I am not sure if this is a questionable use of the metadata format by Adobe or a real issue - so at least for now I worked around by using the BitmapSource metadata which does extract the correct title - I haven't tested but my belief is that the BitmapSource way of doing it is going to be slower and more memory intensive, however extracting the correct title is important enough that any hit is worth it for now.

I had been getting photo titles from the IPTC metadata - this had worked well but I had a problem with some recent files with long titles - the titles were truncated... Initially I thought this was an issue with the metadata extraction since the Bitmapdata from a framework Bitmapsource and Exiftool both showed the Title without issue - but a very helpful comment online - https://github.com/drewnoakes/metadata-extractor/issues/474#issuecomment-626014133 - helped me quickly realize that I had not looked up and did not know this was to Adobe spec and also didn't realize that the generic GetMetadata call into MetadataExtractor didn't return XMP data. The XMP part is critical because that is where the full title is. Added the XMP Title and also some fallback lens info from XMP.

5/7/2020

Unexpected Errors today - I believe these are mostly .NET 3.1 Preview related and some may have to do with errors being exposed that I previously had hidden:
 - Null image source string - I don't believe this should normally error but it did - add the ImageCache converter into Photos (the only spot not using it) and it fixed the problem - also added in a Null Image converter based on https://stackoverflow.com/questions/5399601/imagesourceconverter-error-for-source-null to Utility - not currently using but the behavior in the other converter may not always be ideal
 - BitmapSource not initialized - previously just return a new instance worked - but now it is complaining that it is not initialized, added a small bitmap rather than new bitmap
 - Seeing the Aero2 error - you can continue past this so no big deal, but interesting this is coming up
 - No project Identity and missing Sqlite files - I am trying to stick at preview 3.1 but have had trouble several times with preview EF Core so moved back to stable and everything worked - I suspect some kind of version issue or compatibility with Sqlite
 - AngleSharp is throwing an encoding error - it can be bypassed but seems new with 3.1

The photo auto-save has been great for productivity but it has performance issues if you select too many files at once - added a switch so that at more than 5 files it processes them 1 at a time - worked with large first test batch.

Converted the window screen shot into a user control - to 'fully work' it looks dynamically for the StatusControlContext - this is an internal assumption but a good one for now especially for this 'non core' feature and as a nice way to integrate with low effort.

5/6/2020

Added an initial Screen Shot implementation - originally I had thought to use https://github.com/microsoft/Windows.UI.Composition-Win32-Samples/tree/master/dotnet/WPF/ScreenCapture but after looking more at the sample can Screen Capture docs I didn't understand how to quickly get a single screen shot of just the app window with minimal code - so went back to a traditional Visual render in WPF and rather than figure out that code again went to https://stackoverflow.com/questions/5124825/generating-a-screenshot-of-a-wpf-window. Still need to explore how to get it into each window with minimal hassle (a user control might work - probably can walk the visual tree up to window?).

Did some help text for the common fields - needs reformatting I think but basic idea works - test in Posts atm.

In writing the help realized that the title of a photo is not actually displayed on the photo page - still happy with the current layout that doesn't put it bold or at the top - but the title should be present, so made an overload of the caption to include it. Interestingly for general purpose use I ditched title in caption earlier - but for this specific case I think it makes sense.

A few CSS tweaks for PointlessWaymarks.com

5/5/2020

Tag Exclusions editor v1 done. Updated Tag generation done.

Used a combination of guard statement based on whether the db already exists and schema 'if' in the migration to maybe get a system where I can use the migrations and create new sites with a completely up to date .EnsureCreated db??? Might get too confusing? Or maybe just not worth it?? I think I will run it for now and see what happens.

5/4/2020

With Tags in place I noticed that there were a few tags in photos where I was fine with the tag appearing on the photo page (I think...) but the tag was more personal and I didn't like it showing in the tag list for search. This is easily solved and makes sense to me to have a tags exclusion table in the database. But adding a table would break the apps for previous db version - so time to setup migrations. I initially tried doing this via the EF Core migrations - but quickly ran into trouble since I don't have a single db to point it to - I could change that of course with a always present test db or... but it made me feel like this wasn't quite the scenario they were focused on - so I quickly found FluentMigrator and within a few minutes had it at least seemingly working with the table added!

First version of the Tags Exclusion Editor added - still needs works but basics in place.

Still excited about Fluent Migrations and it working as expected and quickly but now I wondering if I can build a new database with EnsureCreated and the get FluentMigrations to do the right thing... More research needed.

5/3/2020

Initial generation of Tags - an all page with Search and individual pages. Doing this triggered a refactoring in the Search Lists so that any content can potentially be generated (code has to be added for any new types but now not locked to IContentCommon).

5/2/2020

Put change indication in for Notes, Links and Content Type and filled in missing label Targets in those. I believe for now that this takes care of the Label Target and Change Indicator todos.

Added to Todos to look at https://github.com/microsoft/Windows.UI.Composition-Win32-Samples/tree/master/dotnet/WPF/ScreenCapture - this sample is a little to complex for a quick cut and paste but I suspect that the helpers and other information will be overall useful in addition to adding Screen Capture.

5/1/2020

I had moved over to iis for hosting for local dev and had written about it being easier - that was true for some initial work I did but I have now moved back to dotnet serve - in retrospect just as easy I think. Details that helped me -
   - dotnet dev-certs https --help - I didn't know this existed and found it via a google search on https://www.hanselman.com/blog/DevelopingLocallyWithASPNETCoreUnderHTTPSSSLAndSelfSignedCerts.aspx - seems like in many cases the dev certs should just work but I don't think anyone would argue that dev setups have odd hiccups over time esp. if you are constantly installing previews...
   - In dotnet serve you can't forget either -S or -p 443 in conjunction with a Hosts file entry such as 127.0.0.1 pointlesswaymarks.com
   - Probably worth noting ipconfig /flushdns - not sure what situations trigger needing this in which browsers if/when... but worth noting

I now think the problems I had with dotnet serve were actually about some misses doing 'everthing' correctly esp. moving from local debug in http vs https - everything above is documented and simple, but just enough details that I think when new to it I was confused occasionally by not getting everything correct all at once...

Changed the footer to use the core links like in the header and changed the format.

Added new checkboxes and javascript to the All Content List to filter by 'type' - here some content types are grouped together.

4/30/2020

Added a standard header tag - rather than add a responsive hamburger menu just went with a very simple set of text links. If I try migrating HikeLemmon a stronger menu component will be worth looking at but for now playing with simple coding on some menu option I actually wasn't sure anything was a 'win' vs just picking the most important links and showing them. Header also added to a number of additional pages.

Fixed a bug in related content where no related was still showing the related: heading with nothing following.

Added a content type to the content lists data - this is a setup for a future search setup on the 'all' list that allows filtering by type - I think that change will really improve the search.

Did a quick experiment with the TextBox wondering if I replaced the ContextMenu with my own if the Spell Checker would then grab that and use it - it didn't... it is likely that the problem can be solved either in code but I think benefit to time here is to just forget about this for now and let the return stripping in selected stay as a button for now.

4/29/2020

Body Content and Update Notes get visual updates and simple change indication.

Camera Roll updated with links to the Daily Galleries.

Added Camera Roll to the footer.

Fixed an error with new content and the CreatedUpdated control detecting them as not new both in the autofill of updated and in validation.

Worked on more automation for the photo imports.

4/28/2020

Added very basic Bracket code generation helpers and added link commands and buttons into the file editor.

File Content Help updated.

Fixed a bug in the related content generation that didn't include file content - this was obvious on the image pages where they are pdf image pages but weren't linking back to the file content.

Tried the 'Markdown Editor' Visual Studio extension for a week or so - loved the preview but couldn't seem to get spell checking to work so removed it - added Visual Studio Spellchecker in to cover the spell checking.

Published https://pointlesswaymarks.com/Files/PimaCounty/pima-county-multi-species-conservation-plan-2019-annual-report/pima-county-multi-species-conservation-plan-2019-annual-report.html to the main feed - first use of File Content on the index page and it seems to be working but I didn't see immediate rss updates which might need a check...

Updated Index to have the related content - will have to experiment on mobile to see if this works - I think on desktop it is not quite a pure/clean but is reasonable and great for content discovery.

Changed layout of the Camera Roll - after trying with a sort of list of list of days approach decided to go with a single list with date and info items mixed in with the photo items - maybe more 'date' items are needed (maybe years? maybe months?) but overall like this basic approach and also think this approach is a bit more interesting because it creates more difference between the daily galleries and the total list...

4/27/2020

More Help work - currently using the File Editor as a test. It may be worth circling back to see how localized text is commonly done in WPF and see if that is simple enough to try - I didn't go that direction immediately because my first idea was to keep everything 'in code'. Initial results are decent.

Body Content and Update Notes get GridSplitters between the content and preview - this is the most options for the least code so in that sense I love this solution - on my mind are saving and restoring the split position and possibly a shortcut to collapse/restore a side.

Fixed a typo bug in the index page generation when using FileContent in the Index Page.

Created and Updated get the value change info.

4/25/2020

Tags editor adds change detection.

All windows have closing with unsaved changes warnings but the ways tags are evaluated should be improved to be like the tags editor and closing an 'empty' editor is incorrectly flagged as 'has changes'.

Lost an edit by accidentally closing an editor while adding multiple files - as a result started work on detecting changes with the first experiments going into the FileEditorWindow and into the TitleSummarySlug control. Both are fairly simple at this point but in quick testing seem to work. I think the window work can be abstracted slightly to make this slightly faster/easier to get into all of the editors - having abstracted individual property change tracking before I suspect in this case it is simpler and less work to do it as 'one offs'.

The extract links was showing duplicates - eliminated. Also added an option to exclude specified links but not currently in use (left this code in but it didn't work out exactly like I thought - the original idea was to exclude PDFs that were the selected file of a FileContent, but since I often rename the incoming PDF this didn't actually work.

4/23/2020

Added the new from files to the Files interface - not too much code involved and while working on some Saguaro NPS documents found it was less confusing to be able to open all of them at once even if it wasn't any faster to edit.

In the early days of this project I started doing the Command setup in a Load method rather than in the constructor - this was partly to solve some problems I was having (I believe with the MVVMLight RelayCommand - but the earliest work here was copied over from the earliest prerelease .NET Core 3 versions that supported WPF so it could have been something else too) but also I thought it might be valuable to refresh the bindings as Load was called and thought requiring the Command properties to have notification changes like every other property would be nicely consistent - I think the experiment is over though and it didn't work out, needing property changed is just more friction for the commands and I think it this kind of vanilla WPF project it is natural to look for commands in the constructor so starting to walk this decision back - started with the main editors and lists.

Added a helper to get the SelectedText of a TextBox into the View Model - this was for the body content where pasting from PDF was bugging me because the text often copies with copious line breaks that reflect the original formatting nicely but are often imho not relevant for the web. Added a command to remove line breaks from selected text (for what it does the button is unusually prominent in the UI - this was the quickest solution - I added an idea to explore how to extend the textbox content menu and keep existing choices).

Refactored the PDF to Image generation I had added to easily get cover pages - this allowed this code to be included in the file editor (intuitive place for it while adding a new file...) and also allowed an extension to use pdftocairo to quickly make an image of any page number. This came with some complications in file naming but basically works nicely - the file naming complication is that pdftocairo adds a page number to your file name if you are not using 'singlefile' - I don't believe that new file name is included in the output and the exact format is either dependent on total pdf length and/or is not man-page-specified, worked around this but interesting that shelling out this process so quickly ran into issues with this approach, mostly it should not matter but could result in unexpected rare oddities...

4/22/2020

First version of the Camera Roll generation.

Fix a related content miss where photo pages weren't considering 'themselves' when getting daily photo content.

Move newer related content to all pages.

Fix typos in Main Window generation progress loops.

4/21/2020

In running validation on a page I found 2 issues:
 - Missing Doctype tag - fixed - for this content the browsers didn't care as far as I can tell but disappointed I missed this
 - Missing Sizes tag in the srcset images - the first images worked on for the site were all 'full width' (actually constrained but in the spirit of) and I had left out sizes - from reading this seems to be an 'error' but in past experimenting when you were just going to default to 100vw with nothing else in size it seemed that the browsers did the same thing with 100vw and nothing... But now there are the small thumbs and the photo gallery sizes in play - added sizes information. As in the past it seems like one challenge is that while sizes and srcset are incredibly powerful and don't require any javascript it also makes the images resistant to radical on the fly restyling - worth it for now anyway!

These two issues - but especially doctype - emphasize what a value a testing setup would be - perhaps this is possible now that switching setting files is possible? But the challenge is finding something that could be maintained and kept up to date - maybe just auto validation of all output?

Alpha sorted Tags in - I believe - all spots.

Daily Photos links now appearing as related content - Also reworked what appears here with the idea that if you bracket code linked content that it will also be included in the bottom related links which I think emphasizes the available content/resources in a nice way. This required additional bracket and other code to get this working especially in combination with subbing daily photo content for individual photo content.

4/20/2020

Added related content and previous/next to the Daily Photos page and did a little refactoring.

4/19/2020

Basic Daily Photo pages are now generating.

4/18/2020

Start work on the Photo Galleries - starting with daily...

4/14/2020

Improved the startup screen related code so that creating new sites happens much more nicely - I think the settings/settings file could be handled better in general but with this working I think more rework/refactoring waits.

Added the Build Date and GitInfo into the title bar - if this was a commercial project that would be a distraction, un-needed question and probably branding mistake - however because those aren't concerns if you have to ask someone or yourself about 'version' you can't get easier than in the title bar (haha so easier than having to ask is having everything logged and remotely available I suppose, and the version info is in the log but the log is local only so Title bar is still easier...)

Progress in the Initial Load.

Cleanup temporary files - I initially though I would do this via keeping track of each temporary file created - but after thinking about it for a few days decided instead to just look for files older than 2 days and delete those.

4/13/2020

In framework projects I have found the most 'versioning' utility for a desktop app in adding a build timestamp to the published Informational Version and showing that in the title bar. With the move to .NET Core I wanted to look again and try to see if there was something that might be better... I found two approaches to try:
 - https://www.meziantou.net/getting-the-date-of-build-of-a-dotnet-assembly-at-runtime.htm - the build datetime stamp I like so much is maybe a 'version' but maybe it is better as just a specific piece of metadata! This article shows how to quickly/easy add a buildtime attribute - love this, I think it would be more powerful to explore putting it into a Nuget package but for now just added the ?12 lines of code directly to projects.
 - https://github.com/kzu/GitInfo - Writes the Git info into the project on build - for this project where I am not yet doing/managing 'releases' if a friend used the project the best reference would probably be the git commit from the build - seems like this (including if there were uncommitted changes) plus buildtime gives a pretty good picture of what is going on.
 These approaches also leave the true Versions clean for use in either managing releases and/or automated CI style builds which I think is smart.

4/12/2020

Added a default icon and Github social image. Very simple but loads better than the default icon.

First version of a Settings file selector on startup - for now I am not going to support switching settings files on the fly because I am concerned there is too much room for error with scenarios like 'you have an editor window saving when you switch settings files - what happens???', but to get to work on my next project I need to support multiple sites which this does nicely. Only 'new' and 'existing' are supported but put in a placeholder for recent files which I think will be key functionality. Nearly zero testing at this point - this is a first minimal attempt.

4/11/2020

Added Extract Links to all relevant lists - this immediately proved to be very useful since there were links that I thought I had saved that I had not...

4/10/2020

Found an error where I was checking that Photo file names were unique but not slugs - when an image was imported with the same title but a different filename the same folder was used for both pictures - which is not the design and is not going to work in the current setup. Improved the slug checking and added it to other content types also.

Added a check that filenames are unique.

4/9/2020

Image and Photos can now start content from files and starting multiple files is supported - changes for this included option for an initial image in the editor and new code in the Action List.

Fixed a bug with the wrong year parsed from the 19xx photo name pattern - including this is a very personal choice but is such a productivity boost (and likely to not bother anyone else) that I think it is still work including.

Parse a Summary from photos from the title if the description is not present and add Exif description when present. While summary and title being nearly the same is not ideal the idea is to support/help quick photo content creation - basically provide as close to a complete piece of content from the image metadata import and don't have any barriers to providing more if the time and effort is there to do it.

Enhanced the autogeneration of the PDF Images so that an image editor launches with complete enough data to often be immediately saved.

Upgraded to .NET Core 3.1

Added a copy bracket code link to clipboard command to the Image Editor to help with the workflow of generating a cover preview.

Fixed several bugs in the Range Observable Collection with not passing in a concrete list (I am getting an error when that is done).

4/8/2020

Added Jot - https://github.com/anakic/Jot and WpfScreen - https://github.com/micdenny/WpfScreenHelper - and looked at a WPF Sample - https://github.com/microsoft/WPF-Samples/tree/master/Windows/SaveWindowState - and eventually drew heavy and nearly direct inspiration from Markdown Monster to set up tracking and restoring window position - there are so many edge cases that the thought this is perfect is completely laughable but I did try to look at several sources for information and inspiration to quickly put something together that I hope is quite good without unexpected errors and minimal edge cases. As part of this added a manifest file to hopefully ensure the Dpi Aware support is specified by the app - these were the better links that I found but I am not sure I actually understand the when/why/what very well here esp. of what happens by default in what versions and what needs to be setup (probably need to play around with settings esp. when connected to an external monitor to do some real world experimenting and testing...) https://docs.microsoft.com/en-us/windows/communitytoolkit/controls/wpf-winforms/webview, https://github.com/dotnet/wpf/issues/859, https://github.com/microsoft/WPF-Samples/blob/master/PerMonitorDPI/readme.md

Fixed a binding bug that was two way to a read only property in the Link List.

Fixed missing save button command binding in Settings.

Fixed several copy and paste bugs related to the background data updates.

Initial pdftocairo pdf preview generation added.

4/5/2020

Fixed a bug in the Content saves for images and photos where I hadn't quite gotten the recent adjustments to saving/generating right - core detail is that in early versions the files were always an over write operation (vs checking) which meant I didn't want to wait for that every time, but now the standard generate just checks for files rather than overwrites. Downside is if you switch out an image you have to force the regen...

Set up static weak events to serve as a way to alert/update the lists as content changes - light testing in the posts list and it was working nicely!

4/3/2020

Found while working on the Waterman Peak post that I was loading the content editor from the list item without refreshing from the database which had unintended consequences with the Waterman Peak post where I also added images, files and links so was bouncing between editors. Changed in all editors.

Even with the small current amount of content I was noticing that the list filter had some disappointing interaction where you would type and get a bad UI lag. Fixed this by removing the interaction trigger I was originally using in favor of triggering off the property change so that I could also take advantage of the Delay binding options which causes WPF to delay before updating the binding (this seems like insanity but works really well letting you type and the binding not be updated until you pause - which makes the UI appear to be nicely responsive.)

Did some working on setting up all labels with targets - Settings screen done.

First try work on unhandled exceptions - got handlers setup and basic logging in place - need to see which ones I could 'handle', which in this case I think means report and try to resume as an unhandled exception is as likely to be 'error handling bug' as fatal error in the spirit of 'out of memory'. *Changed this up a bit after looking at the Markdown Monster code - this is not the result of extensive testing but rather my impression of the rather solid state of Markdown Monster.

4/1/2020

Added a font size slider to the Body Editor (for me if you make the window full screen the system font size is too small) - also made the refresh preview button a little larger and added a small gap between it and the preview to make it easier to hit.

Added an open file button to the file list to open the document locally - very convenient.

More progress messages especially around local HTML preview generation.

Small changes to the Status Message to give more room to error message display.

Bug Fixes Better handling of Image and File file checking.

3/27/2020

Added some basic progress messaging to the Body Preview construction.

Did a first simple round of work on pushing any navigation out of the WebView preview and into a new window - pretty simple code to start but at least for a the best case this works well.

3/23/2020

Fixed a bug in the Photo Content Editor in the new Media Library/Content Folder check/sync where I didn't have any path for 'first creation' where the file is not yet expected to exist.

Basic Editor visual improvements are done along with basic alt shortcuts - nothing stunning on either but already nice to have a slightly better look and also already using the shortcuts.

3/22/2020

I thought the switch to Spatialite had gone really nicely but had a mystery crash when almost done with the HTML import - https://github.com/dotnet/efcore/issues/14561 - this is a real show stopper where after some number of db operations Spatialite crashes and takes the process with it and when running in debug gives zero info back, I some quick fixes since I am not trying to squeeze performance - VACUUM, disposing and closing connections, pausing for seconds, gc collect... and nothing helped. Sadly for now I have disabled Spatialite and have fallen back to Sqlite. This is probably a wait and see at this point - this is a pretty severe bug so I hope it is worked on, but with so many outstanding issues who knows... One thought I had was to forgo doing any spatial db operations and store spatial data in Sqlite tables and then do operations in memory - this is certainly 2nd rate behind Spatialite but I am wondering if I am running into odd Spatialite issues here - and Sqlite issues nowhere - is it maybe better to do more work in code that I can more easily understand and control vs. relying on Spatialite working 'everywhere' (what would I do in an Android deploy - frankly it would work or not and I would have a difficult time debugging much less fixing any issues, sure same applies to Sqlite but I have had basically no problems so...)

Switched all Content Items to having a ContentVersion that is a UTC DateTime - easy to criticize that this is duplicate data to Created/Updated - I can't totally disagree but I want created and updated to be easy ways to access the human readable times without any hoop jumping related to timezone, so in the end went with the new ContentVersion field to store UTC time. Sqlite doesn't have RowVersion so UTC Time gives an easy unique-enough comparison field where it is also easy to see what was last in. Updated editors and JSON Import.

Json Import Working and used to pull in the current site! With ContentVersion incorporated this feels reasonable and solid - glad to have this option - durability over the long term is hard and while Sqlite is amazing everything goes wrong eventually so nice to have a way to restore from JSON files.

Did some more visual and shortcut key work - will have to work thru more editors and lists but it will be nice to have this in better shape. o intention of high design goals here - just simple improvements.

3/20/2020

Slightly improved the content list visuals - didn't do too much work pulling out resources or styles because I'm not sure at this point if 'more' content types are likely and if they are if they are going to conform well enough to the general pattern...

3/19/2020

In working on the Json import it became obvious it would be a benefit to have some logging. This was a fairly quick project because the GUI Commands and Feedback (and at this point nearly everything is a GUI executed command) run thru the StatusControlContext so code to write useful logging information really only had to go into that class to get a very useful start to ad hoc logging. The logging currently goes to a Sqlite database in the user docs folder. The most interesting detail that I haven't incorporated before is that each StatusControlContext gets a Guid id that is recorded as the 'sender' in the database - it could turn out to be more useful to log a thread id in terms of reconstruction but for now logging the Guid of the StatusControlContext is an easy way to get associated events.

JSON Imports are done with very light testing - T4 generation of the code did end up being a good strategy - easy to debug too. I was concerned about keeping a row version (3/13 log entry) and still think that might be a good idea (and I now think just a UTC DateTime would do it) but leaving that for now and just doing a very simple check on the created/updated fields (this won't help with any time zone type issues but should work nicely for simple 1 user predictable timezone uses).

3/17/2020

A very helpful link of git sparse checkout on windows - glad I came across information on this fairly quickly - https://stackoverflow.com/questions/23289006/on-windows-git-error-sparse-checkout-leaves-no-entry-on-the-working-directory - basically the problem appears to be that if you use powershell (I suspect all versions but wonder about the latest open source cross platform versions) and follow variations of the most common sparse check out recipes the echo [your sparse checkout dir] >> .git/info/sparse-checkout is actually producing Unicode file with a BOM marker - instead you should do something like
 * echo "yourPath" | out-file -encoding ascii .git/info/sparse-checkout
 * Set-Content .git\info\sparse-checkout "someFolder/*" -Encoding Ascii

So a basic recipe of:
```
git init <repo>
cd <repo>
git remote add origin <url>
git config core.sparsecheckout true
Set-Content .git\info\sparse-checkout "SomeSubDir/*" -Encoding Ascii
git pull --depth=1 origin master
```

(Note I typed out this version because it makes sense to me atm - I believe there is now a 'sparse-checkout' command that is at least experimental - there may also be some good details with --filter but I neither immediately understood nor dug into that.)


3/13/2020

Switched DB over to SQLite/Spatialite.

Did some basic code to detect needed but blank settings values and fill them with defaults - this works for now but there is no current way to switch settings file and I do think that will need to be added eventually - current initialization done in the App.xaml constructor.

Wired up the GUI Import and did a first partial import before it broke on the Link imports - it did some imports successfully. Leaving the current commit as a WIP with this broken in part to think about whether I want to change the design to not duplicate anything if you import again. Would the best strategy actually be to move to a row GUID - the current ContentGuid is only a way of connecting the same content - not a way to identify a individual db entity (row...)... This seems like overkill but I wonder if it might be quite interesting for working with a temp local version and bringing over all new content? But you still end up with 'which version is the latest', so is it even worth it?

3/12/2020

Basic Json Import Methods are done - completely untested.

3/11/2020

Changed the model interfaces to be read only - there is some potential lost flexibility in this change but at this point I am comfortable with the 'rule' that you need to use the concrete type if you want to modify the database.

The interface change was largely a realization from working on the json import for the Notes - notes is really the only type that didn't implement all the so-far-needed data interfaces (has a generated title rather than a full 'user' title) which means it is an 'exception'... But by using the NotMapped attribute I was able to added the needed Title and MainPicture properties without storing them in the database (which in retrospect might have been a workable idea - but with this in place I am not going to revisit that at the moment) - this is nice because note now can implement all needed interfaces without any outside mapping needed. 

More Json import generation - historic imports now building - like all json imports 100% untested at the moment.

3/10/2020

Added the first Json imports - some via T4 generated code and Notes as handwritten. Building but not tested.

Added a style for the outer border of the Lists - this is designed to make selected 'obvious enough' without needing to do any indicators.

3/8/2020

Switched some items over to direct https reference - this is 'right' in so many ways but my concern is that I actually don't 100% care about public internet availability for this project - https makes sense for the modern web but I don't want to get in the way of serving a website locally from something like a raspberry pi - I haven't  experimented with this yet but I am starting to be more interested in what in more limited, maybe personal, content - what if a site was only available from my backpack, or my truck - what if a site could truly be turned off, maybe a Mondays only site, not meaning a clever splash screen on other days but literally off - what about a site from your desk literally only running when you were in. I don't know, you can easily think of a million reasons 'not this' or 'it doesn't actually make sense' - but I am curious.

Finished out current Created and Updated Testing.

Better options in the 'General' tab to generate HTML without the Picture Check/Clean and pulled that into a button also.

Added checking in the Link List that selected items are on Pinboard - just checking the URLs not syncing data, but I think the most important is that the URL has been saved since the most core idea is to sync to Pinboard for archiving.

Html Encoding added for quite a bit of meta data.

Added a wpf Behavior to funnel PreviewMouseDown in Readonly Text Boxes to the containing list item for selection - not quite 'perfect' (should perhaps still look at changing this to click) but cleaner than a pure event and at least here defaults to 'more selected' rather than 'less selected' (and better here because unlike like the magic invisible boundaries that defined selection clicks before you can at least understand that selecting text selects the item).

Slight visual improvement in the Link List for selection with just a small margin adjustment.

I had been testing by using dotnet serve - trying to be modern and cool but it turns out that good old iis was setup with zero hassle in about 2 minutes for a simple static site like this - so moving over to that. It would be awesome if this project tested at the push of a button but I think it is better to just use iis (or server of choice) and use time elsewhere.

3/6/2020

Started test project for the Created and Updated string - at first this seemed  contrived but as I did the first two tests I remembered that the number of cases here is actually worth making sure this is right - nice to get this started.

Changed the link list for slightly improved visual and better copy and paste.

Put in some 'ok' code to funnel preview mouse down from the ReadOnly TextBox to get list selection - it would be interesting to see if a mouse input gesture command would work and perhaps be more clear (right now selecting text selects the item - I think it would make more sense it selection triggered on click rather than mouse down?)

3/5/2020

The rendering with the WebView 'always on top' is in fact a known limitation of the control and I believe of XamlIslands in general - in the WPF/Winforms era a similar issue went by the informal name of 'airspace'. At the time one interesting solution basically involved rendering the control into a 'native to the GUi tech' image when you needed to do something on top of it - I don't think that I will explore that and have other UI binding ideas on work arounds... This issue is doesn't have any immediate resolution but at least seems to track this https://github.com/dotnet/runtime/pull/33060

For local use I changed images to inline with Base64 and am copying in rather than trying to link the css. This is actually much nicer than I anticipated - in combo with the rendering in WebView this really imitates the site nicely.

In the WebView the scroll bars appear and cover content - as a simple work around added some margin in by default - at some point this could conflict with the existing CSS and I'm not doing anything to try to detect that but since I am not so sure adding margin to the body is something I am worried about overlapping with this seems like a good enough fix for now.

Add WebView to all current editors.

Add extract link command to all current editors.

3/4/2020

Inserted the WebView into the body editor to disappointing results - two concerns:
 - My first html for the local preview worked so I gave it very little scrutiny, so I was surprised when the images were broken in the first switch out. My formatting was less than perfect so I made some changes but still broken... The issue is documented here by Rick Strahl https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/2211 and the issue was closed as 'by design', basically file:/// urls are not going to work without a IUriToStreamResolver - there is a note that a default implementation is available in a linked commit, but on a quick reading it isn't a public default but rather an internal. I agree with Strahl that this addition hoop is incredibly unwelcome in the context of WPF where you have full file system access...
  - Initial tests seem to show that there may be some Z order (or maybe just 'draw over') issues - I haven't fully verified that but in early tests it visually looks like familiar old problems...
  
So it is not like the old Web Browser control is genius - it is in fact painfully old and you have to work around issues too, but I had hoped for a more impressive start...

(While not used in this project in terms of 'unification/desktop modernization' effort into .NET 5 anyone working on desktop apps should take note of https://github.com/dotnet/runtime/pull/33060 - 'Support COM objects with dynamic keyword' - the inclusion of COM in .NET Core 3 has been painful because of the issue 'Dynamic keyword not working against COM objects' - Rick Strahl - https://github.com/dotnet/runtime/issues/12587 - from early responses for a pull to get so quick to merging so quickly is was a complete surprise to me!)

3/2/2020

I have made several tries at adding the newer MS WebView (soon to be WebView2) that uses an updated rendering engine with no success - Microsoft.Toolkit.Wpf.UI.Controls.WebView - in fact I have worked on sample projects and other experiments where I have also failed. In the early days I ran into code that would run but namespace issues that made everything seem broken. In recent attempts I was stopped because of various library issues including MVVMLight (EventToCommand) and the related change from Microsoft.Xaml.Behaviors.Wpf to System.Windows.Interactivity (the Microsoft.Xaml.Behaviors.Wpf is the open source version of System.Windows.Interactivity) and my use of RelayCommand. WebView is still not in use in the current version but for the first time it installs without issue - details:
 - Switched to Microsoft.Xaml.Behaviors.Wpf - at first I thought this would require rewriting the MVVMLight EventToCommand but is seems so far to be simpler with InvokeCommandAction taking it's place
 - Switched the WPF FontAwesome package to a fork called FontAwesome5 after running into unknown but immediate breakage of the other package with WebView Installed
 - Leaned on the Refractored.MvvmHelpers for Command
 - Finally installed the WebView and was still able to run the app!

2/28/2020

Overwrite true/false support now in the Picture Resizing - this is only done by name, not hash/file size/contents/etc..., with the target really being adding resizing to the 'All Site Html Generation' not as much as perfect protection for changing a photo out on the file system and expecting magic to happen...

Added several methods to check/create the main Directories - this method should be useful when settings switching is added, much nicer to deal with generation if you know you at least tried to generate the folder structure to start.

Put the new methods together into the main form Generate All so that now pictures are fixed as needed.

Did 5 minutes of research on screen resolutions and changed the srcset size list now that it is easy to regenerate - quite slow to generate the image files but not concerned about that at this point and also a little unsure how much time is a result of the often quite large original photo size.

2/27/2020

First work on a slightly better Created and Updated - no test yet so leaving on the todo.

These two relate to better image backup and getting some reporting on missing images - with the html on the site regenerate all rewrites everything so checking for missing might be interesting but isn't currently necessary - images are currently only resized when you save and regen content - that needs to change to make sure all images are available (note that thankfully the site works on the available images - but it you only had the original in the folder that would be a terrible on site experience as it tried to possibly load 30megs of image for a 360px display...):
 - The photo editor on load looks for the original image in the media archive and if it doesn't find it it also looks in the current content directory and copies it back over.
 - The image resize gets a small refactor which makes it more flexible but most importantly it extracts the information that will be needed to evaluate if all the generated sizes are present in a content directory.

2/26/2020

JSON files are exported for each object right beside the html - this is meant as a backup of the raw data - you could argue that a better strategy would be to provide versioned db backups for each deploy - and I do think something like that would be great... But even an insanely well used db like SQLite immediately poses a problem for the lowest level tools because of drivers/format and because one goal of this project is low maintenance high durability that could sit for some time on the .net without attention - if you came back to a project 10 years later how sure are you that you could easily open the SQLite db? Compare to processing text... (Yes JSON has nuances to parsing, but I don't know of a text format to store data that avoids nuances? And working with text in the past I have found the terribleness of a particular format or variation is not nearly as terrible for a restricted set of input vs. the task of 'writing a JSON processor to handle ever use case properly'...).

Change in this version is to regenerate the JSON on each HTML generation.

Better error when you open a photo but the expected Media Archive file isn't found (the 'original file') - originally I just shrugged and thought that with the Media Archive folder easy to backup that you just have to keep it backed up. Now I wonder if I should write original images back and forth into the on folders as needed - but like the JSON with the intent of keeping both?

2/25/2020

The first version of the RSS Feeds was all simple text with MS's SyndicationFeed and Item - quickly worked as expected out of the box. But when I went back to add images and html content I started having issues - part of the issue was certainly inexperience with the ins and outs of that API and now that I have finished out this project for now I do wonder if I circled back to the MS API if I could make it work with what I know now... On the other hand it seems like https://stackoverflow.com/questions/1118409/syndicationfeed-content-as-cdata and https://stackoverflow.com/questions/7204840/rss20feedformatter-ignores-textsyndicationcontent-type-for-syndicationitem-summa indicate that others have struggled with this issue also... Adding to my confusion is that while the RSS Feeds I looked at all seemed to be very consistent in using CDATA at least some RSS Spec example feeds and older info from Nick Bradbury (FeedDemon!) seem to suggest that the CDATA block isn't strictly needed to get an HTML description? So 100% admitting that some of this could be ignorance and confusion but basically:
 - I never found a 100% clean believable up to date seems to work in all readers I tried spec doc - a classic example of my confusion might be the sample file http://static.userland.com/gems/backend/rssTwoExample2.xml from  https://validator.w3.org/feed/docs/rss2.html - Newsblur shows no stories... Newsblur is touchy for sure (I had examples where Feedbin read the feed and Newsblur didn't) but it just needs to work so...
 - There were several work arounds in the StackOverflow questions - I had trouble with all of them and with the original code
So I decided just to go as simple as possible to try to eliminate as much misunderstanding as I could - two string builder methods and a little validation debugging later and it seems to be working as expected. As mentioned above - because I got this working so quickly this way and because of lack of a 'perfect sample' or reference and general confusion I do partly think some of the original solutions (either straight thru the ms api or stackoverflow workarounds) may have worked - but with this easy and working I don't really have any motivation to go back at this point!

Switched the Content/List feeds over to the RSS StringBuilder and refactored that code to a common location.

2/22/2020

Added an Extract New Links for Posts - this should probably expand and maybe become automatic to help you get all your links into the link lists - these are set by default not to appear in the Link RSS feed. Fair comment to note with these links already in content do they need to be in the link list - maybe not but for two reasons: saving links also saves to pinboard if you have an api key set (and this can be incredibly important for archiving) and because in the past I have definitely wanted to just quickly find 'that link' for someone (and while the 'All Content' list/simple search might fail with too much content the link list should be viable for quite some time since it is just divs/links/text...).

Added save and close and open link to the link editor.

Added Dublin Core style metadata to Content Pages.

2/21/2020

When I started looking at pulling all the json backup files in to recreate the database I realized that only their location in the directory structure defined what type was stored in the file (without analyzing the file anyway) - that might be 'ok' but I wanted a better way to tell the contents/type of the file so added more descriptive names. I considered creating a wrapper type that wrote type with nameof into the file but it seemed needless and harder to see than just changing the file names.

I like the auto-saving to Pinboard (most meaningful to an account with auto-archiving...) and thought it would be nice to extract the links from content and bring them up to save as links in the link list (so probably not fully auto at this point anyway) - to support that I added a field to the db structure to indicate whether the link should appear in the Link List RSS (so if you are following the site you are not bombarded twice by the link in a post and then the link in the link list).

I made another quick try at including the new WebView (soon WebView2) control - this time I avoided some initial problems but ended up rolling back because I still ran into System.Interactivity breaking - I believe I have a bit better understanding of which pieces would have to be switched out to use the System.Interactivity replacement but didn't have time to try so rolled back.

2/19/2020

Updated the Link List generation to alter the structures that are generated and to change some classes where I wanted the structure to be different than the compact content.
Changed the compact content generation so that the classes indicate when you have an 'optional' summary (has image - not imagining ever hiding the picture over the summary?) or a non-optional summary. This allowed a quick CSS change to get the desired effect which is good - but it did remind me that it would be interesting to look more at current best practices for css class strategies, I wonder if this should have a been an additional functional class (hide-for-mobile? or can-hide of the optional? or...) instead...

2/17/2020

Added Pinboard integration via a nice wrapper I found in Nuget/GitHub - notes:
 - In addition to 'liking' Pinboard integration is included because Pinboard has a plan where a copy of the of website at the time of the link is stashed - this is important for a long lived project because it is hard to predict what links and sites are going to disappear off the web...
 - A nice part of the Pinboard API is that 'add' has a flag for replace - for this integration to keep it simple I just call Add with the replace and don't worry about whether it existed before or not
 - Rather than separate actions for saving to Pinboard or UI choices I just decided to check for a Pinboard API Token - this could cause confusion if you setup a new config and didn't realize items weren't saving but I don't think that is a major concern at this point and I just put a progress line in.

2/16/2020

AngleSharp Notes:
 - Getting web pages thru the AngleSharp library was pleasantly easy - only hitch was that with the simple sample defaults one of the first pages I hit seemed to detect this style as 'noscript' and some of the metadata wasn't included (I am not even sure this was the intended behaviour, but regardless that was what was happening) but this was quickly fixed by including the AngleSharp.Js package and including .WithJs() - it could be this eventually causes an issue but pleasantly it didn't yet cause any issues or a long enough delay that I cared.
 - I have dealt with Open Graph metadata, and looked at other metadata, for HikeLemmon but hadn't before looked across sites for 'standard metadata' like author and publish date - at least with the sample of sites I looked at, which included many smaller news sites, it was a mess - og: data was often included (I assume in part for Facebook), some sites used https://www.dublincore.org/, some had twitter info, and some didn't have great info. I suppose I understand in terms of business, average consumer and lack of incentives - but this (plus the insane number of script tags included in basically all of these sites) makes me want to do a better job on this with this project - I feel like this data 'should' be there and consistent
