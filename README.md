# ImgurTitleEditor

This application allows you to manage Imgur image uploads easily

# Core features

## Cache

All images and metadata are cached locally.
Only changes are sent to the Imgur API.
It **will not** download your entire gallery every time you open the application,
which makes it suitable for very large galleries.

## Manage Titles and Descriptions

The core feature of this application is to manage image titles and descriptions.
This can no longer be easily done on Imgur itself.
If you have many images, it's worth setting this metadata to allow you to search for images later.

## Image Filter

The main view can filter images. The search is performed against the title, description and file name.
You can also select to only display images with or without a title.

## Mass Actions

Downloading, caching, URL copy, and deleting can be performed on multiple images at once.
A mass uploader makes uploading many images easy.

## Proper OAuth usage

This application properly utilizes the OAuth mechanism of the Imgur API.
This means it doesn't needs to know/store your credentials.
Only a token is saved that you can revoke at any time in your account settings.

# Compiled Binaries

Check the [Releases](https://github.com/AyrA/ImgurTitleEditor/releases) section for compiled binaries,
or use [GitLoad](https://gitload.net/AyrA/ImgurTitleEditor) to browse releases more easily.

# Building

No special requirements.
You can hardcode your own client ID in `Program.cs` if you want.

# How to use

Just double click to run. You will be prompted to authorize.
Once authorized, the cache is built.

# Window Explanation

This chapter explains the different windows of the application

## Authorization Window

This is the first window you are presented with when you have not yet authorized, or when you choose to reauthorize.
Just follow the steps shown on the main area of the form.
You can also reach the settings window from the menu,
if you wish to change your client ID or set the secret before authorizing.

## Settings Window

This allows you to change application settings.

### Client ID

This is the public ID of the client. It's sent during the authorization phase.
The default ID should work as I registered it for this specific Application.

### Client Secret

This is used to renew the client ID.
It's not needed for the application to work but you will not be able to renew your token.
This means once the authorization expires, you have to manually do it again.

The secret is usually hidden, but can be revealed by setting the input focus into the text area.

### Page Size

This defines how many images are shown on each page.
50 is the default as it's what Imgur would use too.
You can set this to any positive number.
Numbers of 200 and above will start to show serious lag when the list is first rendered.
Setting it to 0 renders all images on a single page.
I have about 1500 images and this takes about 4 seconds if all thumbnails are cached.

### Register Button

This opens the Imgur URL to register.
You need to be logged into your account in your default browser for it to work.
Next to the button is a link to copy the URL to the clipboard if you prefer it this way.

Registering an application is optional. You need to do it if you want a client secret.

## Cache Builder

The cache builder is automatically opened once you are authorized.
It will download all image metadata and then all thumbnails.
**It's highly recommended that you let it finish or you will experience extremely sluggish performance.**

You can also open the cache builder from the main window file menu at any time.
This is recommended if you uploaded images outside of this application,
or if you performed any other operation on them (for example setting a description) outside of this application.

## Main Window

This is the window you are presented with once you are authorized.
It mainly consists of your image list.
Common keyboard combinations work:

- `[CTRL]`+`[A]` Select everything
- `[CTRL]`+`[C]` Copy URLs to clipboard
- `[CTRL]`+`[S]` Download images
- `[DEL]` Delete images with confirmation
- `[ENTER]` (or double click) Edit title and description

The list also features a context menu for common actions.

The buttons on the top allow you to go through the pages. (See page size in settings window)

## Property Window

The property window can be opened by double clicking on an image.
It allows you to quickly edit description and title of an image.
Pressing `[ENTER]` in the title box or `[CTRL]`+`[ENTER]` in the description box will save immedately.

Using the `[<<]` and `[>>]` buttons will save your changes (if any) and advance to the next/previous image.
Once you do this, the form will remember this action, and until you close it,
the text box hotkeys mentioned above will perform this action too rather than closing the form.
The paging mechanism respects the current filter setup in the main window.

## Upload Window

While it's not the main purpose of this application,
you can upload images and supported video files to Imgur.

Select the image you want, enter title and description, then press "Upload".

Alternatively, click the "Clipboard" button to upload the image in your clipboard.
In that case you can set a file name too if you wish but it's not necessary.

You can also initiate an upload by dragging an image onto the application.

## Bulk Uploader

The bulk uploader allows you to upload many images and optionally assign them to an album.

The bottom text box allows you to set the title assigned to the uploaded images.
The variables %N and %X can be used as placeholder for the file name and extension.
It defaults to `%N.%X` which is essentially the name as displayed by windows explorer (without the path)

The uploader closes automatically after all uploads are completed

### Rate limits

The Imgur API enforces a few rate limits. If you upload more than 10 to 20 images you might run into this problem.
If the upload of files stops (controls become editable again) but there are still images left in the list,
try to upload again in a few minutes.

You do not need to add the images that were already removed from the list again.
Images that were removed were successfully uploaded and added to the album.

# License and Terms

## Application

This application is licensed under the MIT.

## Imgur API

Imgur doesn't allows you to use their API commercially.
Please contact them if you want to commercially distribute/use this application.
If you plan to commercially distribute this application you likely will need to change the Icon too.

## Imgur and Logo

Imgur and the Imgur logo are a trademark of Imgur Inc.
Imgur Inc. is in no way affiliated with the creator of ImgurTitleEditor
