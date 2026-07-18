namespace Flare.Components;

/// <summary>
/// Flare's built-in, dependency-free SVG icon set. Each member is a ready <see cref="FlareSvgIcon"/> that
/// renders inline (no icon font, no network request, no FOUT), so it works under any theme and backs the
/// default chrome of Flare components. Use anywhere a <see cref="FlareIcon"/> is accepted, e.g.
/// <c>&lt;FlareIconButton Icon="@FlareIcons.Home" /&gt;</c>.
/// </summary>
/// <remarks>Icon artwork is derived from the Material Symbols set (Apache License 2.0); see the repository NOTICE.</remarks>
public static class FlareIcons
{
    /// <summary>The built-in <c>add</c> icon.</summary>
    public static FlareSvgIcon Add { get; } = new() { Data = "M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z" };
    /// <summary>The built-in <c>edit</c> icon.</summary>
    public static FlareSvgIcon Edit { get; } = new() { Data = "M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04c.39-.39.39-1.02 0-1.41l-2.34-2.34c-.39-.39-1.02-.39-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z" };
    /// <summary>The built-in <c>delete</c> icon.</summary>
    public static FlareSvgIcon Delete { get; } = new() { Data = "M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z" };
    /// <summary>The built-in <c>save</c> icon.</summary>
    public static FlareSvgIcon Save { get; } = new() { Data = "M17 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V7l-4-4zm-5 16c-1.66 0-3-1.34-3-3s1.34-3 3-3 3 1.34 3 3-1.34 3-3 3zm3-10H5V5h10v4z" };
    /// <summary>The built-in <c>share</c> icon.</summary>
    public static FlareSvgIcon Share { get; } = new() { Data = "M18 16.08c-.76 0-1.44.3-1.96.77L8.91 12.7c.05-.23.09-.46.09-.7s-.04-.47-.09-.7l7.05-4.11c.54.5 1.25.81 2.04.81 1.66 0 3-1.34 3-3s-1.34-3-3-3-3 1.34-3 3c0 .24.04.47.09.7L8.04 9.81C7.5 9.31 6.79 9 6 9c-1.66 0-3 1.34-3 3s1.34 3 3 3c.79 0 1.5-.31 2.04-.81l7.12 4.16c-.05.21-.08.43-.08.65 0 1.61 1.31 2.92 2.92 2.92 1.61 0 2.92-1.31 2.92-2.92s-1.31-2.92-2.92-2.92z" };
    /// <summary>The built-in <c>send</c> icon.</summary>
    public static FlareSvgIcon Send { get; } = new() { Data = "M2.01 21L23 12 2.01 3 2 10l15 2-15 2z" };
    /// <summary>The built-in <c>reply</c> icon.</summary>
    public static FlareSvgIcon Reply { get; } = new() { Data = "M10 9V5l-7 7 7 7v-4.1c5 0 8.5 1.6 11 5.1-1-5-4-10-11-11z" };
    /// <summary>The built-in <c>forward</c> icon.</summary>
    public static FlareSvgIcon Forward { get; } = new() { Data = "M14 9l7 7-7 7v-4.1c-5 0-8.5 1.6-11 5.1 1-5 4-10 11-11V9z" };
    /// <summary>The built-in <c>download</c> icon.</summary>
    public static FlareSvgIcon Download { get; } = new() { Data = "M19 9h-4V3H9v6H5l7 7 7-7zM5 18v2h14v-2H5z" };
    /// <summary>The built-in <c>upload</c> icon.</summary>
    public static FlareSvgIcon Upload { get; } = new() { Data = "M9 16h6v-6h4l-7-7-7 7h4zm-4 2h14v2H5z" };
    /// <summary>The built-in <c>print</c> icon.</summary>
    public static FlareSvgIcon Print { get; } = new() { Data = "M19 8H5c-1.66 0-3 1.34-3 3v6h4v4h12v-4h4v-6c0-1.66-1.34-3-3-3zm-3 11H8v-5h8v5zm3-7c-.55 0-1-.45-1-1s.45-1 1-1 1 .45 1 1-.45 1-1 1zm-1-9H6v4h12V3z" };
    /// <summary>The built-in <c>refresh</c> icon.</summary>
    public static FlareSvgIcon Refresh { get; } = new() { Data = "M17.65 6.35C16.2 4.9 14.21 4 12 4c-4.42 0-7.99 3.58-7.99 8s3.57 8 7.99 8c3.73 0 6.84-2.55 7.73-6h-2.08c-.82 2.33-3.04 4-5.65 4-3.31 0-6-2.69-6-6s2.69-6 6-6c1.66 0 3.14.69 4.22 1.78L13 11h7V4l-2.35 2.35z" };
    /// <summary>The built-in <c>tune</c> icon.</summary>
    public static FlareSvgIcon Tune { get; } = new() { Data = "M3 17v2h6v-2H3zM3 5v2h10V5H3zm10 16v-2h8v-2h-8v-2h-2v6h2zM7 9v2H3v2h4v2h2V9H7zm14 4v-2H11v2h10zm-6-4h2V7h4V5h-4V3h-2v6z" };
    /// <summary>The built-in <c>search</c> icon.</summary>
    public static FlareSvgIcon Search { get; } = new() { Data = "M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z" };
    /// <summary>The built-in <c>filter_list</c> icon.</summary>
    public static FlareSvgIcon FilterList { get; } = new() { Data = "M10 18h4v-2h-4v2zM3 6v2h18V6H3zm3 7h12v-2H6v2z" };
    /// <summary>The built-in <c>sort</c> icon.</summary>
    public static FlareSvgIcon Sort { get; } = new() { Data = "M3 18h6v-2H3v2zM3 6v2h18V6H3zm0 7h12v-2H3v2z" };
    /// <summary>The built-in <c>drag_handle</c> icon.</summary>
    public static FlareSvgIcon DragHandle { get; } = new() { Data = "M20 9H4v2h16V9zM4 15h16v-2H4v2z" };
    /// <summary>The built-in <c>copy_all</c> icon.</summary>
    public static FlareSvgIcon CopyAll { get; } = new() { Data = "M4 5h2v2H4zm0 4h2v2H4zm0 4h2v2H4zm4-8h12v2H8zm0 4h12v2H8zm0 4h12v2H8zM2 19h12v2H2zm14 0h6v2h-6z" };
    /// <summary>The built-in <c>open_in_new</c> icon.</summary>
    public static FlareSvgIcon OpenInNew { get; } = new() { Data = "M19 19H5V5h7V3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2v-7h-2v7zM14 3v2h3.59l-9.83 9.83 1.41 1.41L19 6.41V10h2V3h-7z" };
    /// <summary>The built-in <c>home</c> icon.</summary>
    public static FlareSvgIcon Home { get; } = new() { Data = "M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z" };
    /// <summary>The built-in <c>menu</c> icon.</summary>
    public static FlareSvgIcon Menu { get; } = new() { Data = "M3 18h18v-2H3v2zm0-5h18v-2H3v2zm0-7v2h18V6H3z" };
    /// <summary>The built-in <c>arrow_back</c> icon.</summary>
    public static FlareSvgIcon ArrowBack { get; } = new() { Data = "M20 11H7.83l5.59-5.59L12 4l-8 8 8 8 1.41-1.41L7.83 13H20v-2z" };
    /// <summary>The built-in <c>arrow_forward</c> icon.</summary>
    public static FlareSvgIcon ArrowForward { get; } = new() { Data = "M12 4l-1.41 1.41L16.17 11H4v2h12.17l-5.58 5.59L12 20l8-8z" };
    /// <summary>The built-in <c>arrow_drop_down</c> icon.</summary>
    public static FlareSvgIcon ArrowDropDown { get; } = new() { Data = "M7 10l5 5 5-5z" };
    /// <summary>The built-in <c>arrow_drop_up</c> icon.</summary>
    public static FlareSvgIcon ArrowDropUp { get; } = new() { Data = "M7 14l5-5 5 5z" };
    /// <summary>The built-in <c>chevron_right</c> icon.</summary>
    public static FlareSvgIcon ChevronRight { get; } = new() { Data = "M10 6L8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6z" };
    /// <summary>The built-in <c>chevron_left</c> icon.</summary>
    public static FlareSvgIcon ChevronLeft { get; } = new() { Data = "M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z" };
    /// <summary>The built-in <c>expand_more</c> icon.</summary>
    public static FlareSvgIcon ExpandMore { get; } = new() { Data = "M16.59 8.59L12 13.17 7.41 8.59 6 10l6 6 6-6z" };
    /// <summary>The built-in <c>expand_less</c> icon.</summary>
    public static FlareSvgIcon ExpandLess { get; } = new() { Data = "M12 8l-6 6 1.41 1.41L12 10.83l4.59 4.58L18 14z" };
    /// <summary>The built-in <c>first_page</c> icon.</summary>
    public static FlareSvgIcon FirstPage { get; } = new() { Data = "M18.41 16.59L13.82 12l4.59-4.59L17 6l-6 6 6 6zM6 6h2v12H6z" };
    /// <summary>The built-in <c>last_page</c> icon.</summary>
    public static FlareSvgIcon LastPage { get; } = new() { Data = "M5.59 7.41L10.18 12l-4.59 4.59L7 18l6-6-6-6zM16 6h2v12h-2z" };
    /// <summary>The built-in <c>more_vert</c> icon.</summary>
    public static FlareSvgIcon MoreVert { get; } = new() { Data = "M12 8c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zm0 2c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2zm0 6c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2z" };
    /// <summary>The built-in <c>more_horiz</c> icon.</summary>
    public static FlareSvgIcon MoreHoriz { get; } = new() { Data = "M6 10c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2zm12 0c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2zm-6 0c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2z" };
    /// <summary>The built-in <c>info</c> icon.</summary>
    public static FlareSvgIcon Info { get; } = new() { Data = "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z" };
    /// <summary>The built-in <c>warning</c> icon.</summary>
    public static FlareSvgIcon Warning { get; } = new() { Data = "M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z" };
    /// <summary>The built-in <c>error</c> icon.</summary>
    public static FlareSvgIcon Error { get; } = new() { Data = "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z" };
    /// <summary>The built-in <c>check</c> icon.</summary>
    public static FlareSvgIcon Check { get; } = new() { Data = "M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z" };
    /// <summary>The built-in <c>check_circle</c> icon.</summary>
    public static FlareSvgIcon CheckCircle { get; } = new() { Data = "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z" };
    /// <summary>The built-in <c>cancel</c> icon.</summary>
    public static FlareSvgIcon Cancel { get; } = new() { Data = "M12 2C6.47 2 2 6.47 2 12s4.47 10 10 10 10-4.47 10-10S17.53 2 12 2zm5 13.59L15.59 17 12 13.41 8.41 17 7 15.59 10.59 12 7 8.41 8.41 7 12 10.59 15.59 7 17 8.41 13.41 12 17 15.59z" };
    /// <summary>The built-in <c>done_all</c> icon.</summary>
    public static FlareSvgIcon DoneAll { get; } = new() { Data = "M18 7l-1.41-1.41-6.34 6.34 1.41 1.41L18 7zm4.24-1.41L11.66 16.17 7.48 12l-1.41 1.41L11.66 19l12-12-1.42-1.41zM.41 13.41L6 19l1.41-1.41L1.83 12 .41 13.41z" };
    /// <summary>The built-in <c>person</c> icon.</summary>
    public static FlareSvgIcon Person { get; } = new() { Data = "M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" };
    /// <summary>The built-in <c>group</c> icon.</summary>
    public static FlareSvgIcon Group { get; } = new() { Data = "M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5c-1.66 0-3 1.34-3 3s1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5C6.34 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z" };
    /// <summary>The built-in <c>email</c> icon.</summary>
    public static FlareSvgIcon Email { get; } = new() { Data = "M20 4H4c-1.1 0-1.99.9-1.99 2L2 18c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4l-8 5-8-5V6l8 5 8-5v2z" };
    /// <summary>The built-in <c>phone</c> icon.</summary>
    public static FlareSvgIcon Phone { get; } = new() { Data = "M6.62 10.79c1.44 2.83 3.76 5.14 6.59 6.59l2.2-2.2c.27-.27.67-.36 1.02-.24 1.12.37 2.33.57 3.57.57.55 0 1 .45 1 1V20c0 .55-.45 1-1 1-9.39 0-17-7.61-17-17 0-.55.45-1 1-1h3.5c.55 0 1 .45 1 1 0 1.25.2 2.45.57 3.57.11.35.03.74-.25 1.02l-2.2 2.2z" };
    /// <summary>The built-in <c>notifications</c> icon.</summary>
    public static FlareSvgIcon Notifications { get; } = new() { Data = "M12 22c1.1 0 2-.9 2-2h-4c0 1.1.9 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z" };
    /// <summary>The built-in <c>inbox</c> icon.</summary>
    public static FlareSvgIcon Inbox { get; } = new() { Data = "M19 3H4.99c-1.11 0-1.98.89-1.98 2L3 19c0 1.1.88 2 1.99 2H19c1.1 0 2-.9 2-2V5c0-1.11-.9-2-2-2zm0 12h-4c0 1.66-1.35 3-3 3s-3-1.34-3-3H4.99V5H19v10z" };
    /// <summary>The built-in <c>folder</c> icon.</summary>
    public static FlareSvgIcon Folder { get; } = new() { Data = "M10 4H4c-1.1 0-1.99.9-1.99 2L2 18c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V8c0-1.1-.9-2-2-2h-8l-2-2z" };
    /// <summary>The built-in <c>folder_open</c> icon.</summary>
    public static FlareSvgIcon FolderOpen { get; } = new() { Data = "M20 6h-8l-2-2H4c-1.1 0-1.99.9-1.99 2L2 18c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V8c0-1.1-.9-2-2-2zm0 12H4V8h16v10z" };
    /// <summary>The built-in <c>file_copy</c> icon.</summary>
    public static FlareSvgIcon FileCopy { get; } = new() { Data = "M16 1H4c-1.1 0-2 .9-2 2v14h2V3h12V1zm3 4H8c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h11c1.1 0 2-.9 2-2V7c0-1.1-.9-2-2-2zm0 16H8V7h11v14z" };
    /// <summary>The built-in <c>attach_file</c> icon.</summary>
    public static FlareSvgIcon AttachFile { get; } = new() { Data = "M16.5 6v11.5c0 2.21-1.79 4-4 4s-4-1.79-4-4V5c0-1.38 1.12-2.5 2.5-2.5s2.5 1.12 2.5 2.5v10.5c0 .55-.45 1-1 1s-1-.45-1-1V6H10v9.5c0 1.38 1.12 2.5 2.5 2.5s2.5-1.12 2.5-2.5V5c0-2.21-1.79-4-4-4S7 2.79 7 5v12.5c0 3.04 2.46 5.5 5.5 5.5s5.5-2.46 5.5-5.5V6h-1.5z" };
    /// <summary>The built-in <c>link</c> icon.</summary>
    public static FlareSvgIcon Link { get; } = new() { Data = "M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z" };
    /// <summary>The built-in <c>image</c> icon.</summary>
    public static FlareSvgIcon Image { get; } = new() { Data = "M21 19V5c0-1.1-.9-2-2-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2zM8.5 13.5l2.5 3.01L14.5 12l4.5 6H5l3.5-4.5z" };
    /// <summary>The built-in <c>photo_camera</c> icon.</summary>
    public static FlareSvgIcon PhotoCamera { get; } = new() { Data = "M12 15.2c1.77 0 3.2-1.43 3.2-3.2S13.77 8.8 12 8.8 8.8 10.23 8.8 12s1.43 3.2 3.2 3.2zM9 2L7.17 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2h-3.17L15 2H9zm3 15c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5z" };
    /// <summary>The built-in <c>settings</c> icon.</summary>
    public static FlareSvgIcon Settings { get; } = new() { Data = "M19.14 12.94c.04-.3.06-.61.06-.94 0-.32-.02-.64-.07-.94l2.03-1.58c.18-.14.23-.41.12-.61l-1.92-3.32c-.12-.22-.37-.29-.59-.22l-2.39.96c-.5-.38-1.03-.7-1.62-.94l-.36-2.54c-.04-.24-.24-.41-.48-.41h-3.84c-.24 0-.43.17-.47.41l-.36 2.54c-.59.24-1.13.57-1.62.94l-2.39-.96c-.22-.08-.47 0-.59.22L2.74 8.87c-.12.21-.08.47.12.61l2.03 1.58c-.05.3-.09.63-.09.94s.02.64.07.94l-2.03 1.58c-.18.14-.23.41-.12.61l1.92 3.32c.12.22.37.29.59.22l2.39-.96c.5.38 1.03.7 1.62.94l.36 2.54c.05.24.24.41.48.41h3.84c.24 0 .44-.17.47-.41l.36-2.54c.59-.24 1.13-.56 1.62-.94l2.39.96c.22.08.47 0 .59-.22l1.92-3.32c.12-.22.07-.47-.12-.61l-2.01-1.58zM12 15.6c-1.98 0-3.6-1.62-3.6-3.6s1.62-3.6 3.6-3.6 3.6 1.62 3.6 3.6-1.62 3.6-3.6 3.6z" };
    /// <summary>The built-in <c>favorite</c> icon.</summary>
    public static FlareSvgIcon Favorite { get; } = new() { Data = "M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 5.42 4.42 3 7.5 3c1.74 0 3.41.81 4.5 2.09C13.09 3.81 14.76 3 16.5 3 19.58 3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z" };
    /// <summary>The built-in <c>star</c> icon.</summary>
    public static FlareSvgIcon Star { get; } = new() { Data = "M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" };
    /// <summary>The built-in <c>visibility</c> icon.</summary>
    public static FlareSvgIcon Visibility { get; } = new() { Data = "M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z" };
    /// <summary>The built-in <c>visibility_off</c> icon.</summary>
    public static FlareSvgIcon VisibilityOff { get; } = new() { Data = "M12 7c2.76 0 5 2.24 5 5 0 .65-.13 1.26-.36 1.83l2.92 2.92c1.51-1.26 2.7-2.89 3.43-4.75-1.73-4.39-6-7.5-11-7.5-1.4 0-2.74.25-3.98.7l2.16 2.16C10.74 7.13 11.35 7 12 7zM2 4.27l2.28 2.28.46.46C3.08 8.3 1.78 10.02 1 12c1.73 4.39 6 7.5 11 7.5 1.55 0 3.03-.3 4.38-.84l.42.42L19.73 22 21 20.73 3.27 3 2 4.27zM7.53 9.8l1.55 1.55c-.05.21-.08.43-.08.65 0 1.66 1.34 3 3 3 .22 0 .44-.03.65-.08l1.55 1.55c-.67.33-1.41.53-2.2.53-2.76 0-5-2.24-5-5 0-.79.2-1.53.53-2.2zm4.31-.78l3.15 3.15.02-.16c0-1.66-1.34-3-3-3l-.17.01z" };
    /// <summary>The built-in <c>lock</c> icon.</summary>
    public static FlareSvgIcon Lock { get; } = new() { Data = "M18 8h-1V6c0-2.76-2.24-5-5-5S7 3.24 7 6v2H6c-1.1 0-2 .9-2 2v10c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V10c0-1.1-.9-2-2-2zm-6 9c-1.1 0-2-.9-2-2s.9-2 2-2 2 .9 2 2-.9 2-2 2zm3.1-9H8.9V6c0-1.71 1.39-3.1 3.1-3.1 1.71 0 3.1 1.39 3.1 3.1v2z" };
    /// <summary>The built-in <c>calendar_today</c> icon.</summary>
    public static FlareSvgIcon CalendarToday { get; } = new() { Data = "M20 3h-1V1h-2v2H7V1H5v2H4c-1.1 0-2 .9-2 2v16c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 18H4V8h16v13z" };
    /// <summary>The built-in <c>schedule</c> icon.</summary>
    public static FlareSvgIcon Schedule { get; } = new() { Data = "M11.99 2C6.47 2 2 6.48 2 12s4.47 10 9.99 10C17.52 22 22 17.52 22 12S17.52 2 11.99 2zM12 20c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8zm.5-13H11v6l5.25 3.15.75-1.23-4.5-2.67V7z" };
    /// <summary>The built-in <c>thumb_up</c> icon.</summary>
    public static FlareSvgIcon ThumbUp { get; } = new() { Data = "M1 21h4V9H1v12zm22-11c0-1.1-.9-2-2-2h-6.31l.95-4.57.03-.32c0-.41-.17-.79-.44-1.06L14.17 1 7.59 7.59C7.22 7.95 7 8.45 7 9v10c0 1.1.9 2 2 2h9c.83 0 1.54-.5 1.84-1.22l3.02-7.05c.09-.23.14-.47.14-.73v-2z" };
    /// <summary>The built-in <c>thumb_down</c> icon.</summary>
    public static FlareSvgIcon ThumbDown { get; } = new() { Data = "M15 3H6c-.83 0-1.54.5-1.84 1.22l-3.02 7.05c-.09.23-.14.47-.14.73v2c0 1.1.9 2 2 2h6.31l-.95 4.57-.03.32c0 .41.17.79.44 1.06L10.83 23l6.59-6.59c.36-.36.58-.86.58-1.41V5c0-1.1-.9-2-2-2zm4 0v12h4V3h-4z" };
    /// <summary>The built-in <c>close</c> icon.</summary>
    public static FlareSvgIcon Close { get; } = new() { Data = "M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z" };
    /// <summary>The built-in <c>dark_mode</c> icon.</summary>
    public static FlareSvgIcon DarkMode { get; } = new() { Data = "M12 3c-4.97 0-9 4.03-9 9s4.03 9 9 9 9-4.03 9-9c0-.46-.04-.92-.1-1.36-.98 1.37-2.58 2.26-4.4 2.26-2.98 0-5.4-2.42-5.4-5.4 0-1.81.89-3.42 2.26-4.4-.44-.06-.9-.1-1.36-.1z" };
    /// <summary>The built-in <c>light_mode</c> icon.</summary>
    public static FlareSvgIcon LightMode { get; } = new() { Data = "M6.76 4.84l-1.8-1.79-1.41 1.41 1.79 1.79 1.42-1.41zM4 10.5H1v2h3v-2zm9-9.95h-2V3.5h2V.55zm7.45 3.91l-1.41-1.41-1.79 1.79 1.41 1.41 1.79-1.79zm-3.21 13.7l1.79 1.8 1.41-1.41-1.8-1.79-1.4 1.4zM20 10.5v2h3v-2h-3zm-8-5c-3.31 0-6 2.69-6 6s2.69 6 6 6 6-2.69 6-6-2.69-6-6-6zm-1 16.95h2V19.5h-2v2.95zm-7.45-3.91l1.41 1.41 1.79-1.8-1.41-1.41-1.79 1.8z" };
    /// <summary>The built-in <c>palette</c> icon.</summary>
    public static FlareSvgIcon Palette { get; } = new() { Data = "M12 3c-4.97 0-9 4.03-9 9s4.03 9 9 9c.83 0 1.5-.67 1.5-1.5 0-.39-.15-.74-.39-1.01-.23-.26-.38-.61-.38-.99 0-.83.67-1.5 1.5-1.5H16c2.76 0 5-2.24 5-5 0-4.42-4.03-8-9-8zm-5.5 9c-.83 0-1.5-.67-1.5-1.5S5.67 9 6.5 9 8 9.67 8 10.5 7.33 12 6.5 12zm3-4C8.67 8 8 7.33 8 6.5S8.67 5 9.5 5s1.5.67 1.5 1.5S10.33 8 9.5 8zm5 0c-.83 0-1.5-.67-1.5-1.5S13.67 5 14.5 5s1.5.67 1.5 1.5S15.33 8 14.5 8zm3 4c-.83 0-1.5-.67-1.5-1.5S16.67 9 17.5 9s1.5.67 1.5 1.5-.67 1.5-1.5 1.5z" };
    /// <summary>The built-in <c>code</c> icon.</summary>
    public static FlareSvgIcon Code { get; } = new() { Data = "M9.4 16.6L4.8 12l4.6-4.6L8 6l-6 6 6 6 1.4-1.4zm5.2 0l4.6-4.6-4.6-4.6L16 6l6 6-6 6-1.4-1.4z" };
    /// <summary>The built-in <c>bug_report</c> icon.</summary>
    public static FlareSvgIcon BugReport { get; } = new() { Data = "M20 8h-2.81c-.45-.78-1.07-1.45-1.82-1.96L17 4.41 15.59 3l-2.17 2.17C12.96 5.06 12.49 5 12 5c-.49 0-.96.06-1.41.17L8.41 3 7 4.41l1.62 1.63C7.88 6.55 7.26 7.22 6.81 8H4v2h2.09c-.05.33-.09.66-.09 1v1H4v2h2v1c0 .34.04.67.09 1H4v2h2.81c1.04 1.79 2.97 3 5.19 3s4.15-1.21 5.19-3H20v-2h-2.09c.05-.33.09-.66.09-1v-1h2v-2h-2v-1c0-.34-.04-.67-.09-1H20V8zm-6 8h-4v-2h4v2zm0-4h-4v-2h4v2z" };
    /// <summary>The built-in <c>rocket_launch</c> icon.</summary>
    public static FlareSvgIcon RocketLaunch { get; } = new() { Data = "M9.19 6.35c-2.04 2.29-3.44 5.58-3.57 5.89L2 10.69l4.05-4.05c.06-.06.28-.26.68-.37L9.19 6.35zm.82 11.46l-1.56-1.56-1.56-1.56 2.5-1.62c.3-.13 3.59-1.53 5.89-3.57l-.11 2.46c-.11.4-.31.62-.37.68l-4.79 5.17zm11.99-11.1c.06 2.37-1.17 4.79-3.27 6.53l-4.4-4.4c1.74-2.1 4.16-3.33 6.53-3.27.14 1.36.18 1.27.17.14l-.03 1zm-10 7.84l-1.52-1.52 4.4 4.4zm-8.5 3.41l2.11-2.11 1.41 1.41-2.11 2.11L3.5 17.96zm9.5-3.5c-.55 0-1-.45-1-1s.45-1 1-1 1 .45 1 1-.45 1-1 1z" };
    /// <summary>The built-in <c>local_fire_department</c> icon.</summary>
    public static FlareSvgIcon LocalFireDepartment { get; } = new() { Data = "M12 23c4.97 0 9-4.03 9-9 0-4.59-3.13-8.43-7.37-9.53C15.38 6.25 16 8.06 16 10c0 2.21-1.79 4-4 4s-4-1.79-4-4c0-.78.23-1.51.62-2.13C6.88 9.37 6 11.08 6 13c0 3.87 2.69 7.12 6.3 7.87L12 21v2zm0-4c-1.66 0-3-1.34-3-3 0-.95.45-1.8 1.14-2.34.65.98 1.74 1.67 3 1.67 1.1 0 2.07-.48 2.74-1.25.17.3.26.64.26 1 0 1.66-1.34 3-3.14 3z" };
    /// <summary>The built-in <c>flash_on</c> icon.</summary>
    public static FlareSvgIcon FlashOn { get; } = new() { Data = "M7 2v11h3v9l7-12h-4l4-8z" };
    /// <summary>The built-in <c>speed</c> icon.</summary>
    public static FlareSvgIcon Speed { get; } = new() { Data = "M20.38 8.57l-1.23 1.85a8 8 0 0 1-.22 7.58H5.07A8 8 0 0 1 15.58 6.85l1.85-1.23A10 10 0 0 0 3.35 19a2 2 0 0 0 1.72 1h13.85a2 2 0 0 0 1.74-1 10 10 0 0 0-.27-10.44zm-9.79 6.84a2 2 0 0 0 2.83 0l5.66-8.49-8.49 5.66a2 2 0 0 0 0 2.83z" };

    // The icon ids (snake_case), matching the built-in members above.
    private static readonly string[] _names =
    [
        "add",
        "edit",
        "delete",
        "save",
        "share",
        "send",
        "reply",
        "forward",
        "download",
        "upload",
        "print",
        "refresh",
        "tune",
        "search",
        "filter_list",
        "sort",
        "drag_handle",
        "copy_all",
        "open_in_new",
        "home",
        "menu",
        "arrow_back",
        "arrow_forward",
        "arrow_drop_down",
        "arrow_drop_up",
        "chevron_right",
        "chevron_left",
        "expand_more",
        "expand_less",
        "first_page",
        "last_page",
        "more_vert",
        "more_horiz",
        "info",
        "warning",
        "error",
        "check",
        "check_circle",
        "cancel",
        "done_all",
        "person",
        "group",
        "email",
        "phone",
        "notifications",
        "inbox",
        "folder",
        "folder_open",
        "file_copy",
        "attach_file",
        "link",
        "image",
        "photo_camera",
        "settings",
        "favorite",
        "star",
        "visibility",
        "visibility_off",
        "lock",
        "calendar_today",
        "schedule",
        "thumb_up",
        "thumb_down",
        "close",
        "dark_mode",
        "light_mode",
        "palette",
        "code",
        "bug_report",
        "rocket_launch",
        "local_fire_department",
        "flash_on",
        "speed",
    ];

    /// <summary>All built-in icon ids, sorted alphabetically.</summary>
    public static IEnumerable<string> All => _names.OrderBy(n => n);

    /// <summary>Brand marks shipped with Flare.</summary>
    public static class Brands
    {
        /// <summary>Raw SVG inner markup of the Flare brand mark (a stylized "F"), authored on a 1024 grid and
        /// scaled into the shared 24x24 icon viewBox. Pass to <see cref="FlareIconView"/>'s <c>Icon</c>.</summary>
        public const string FlareLogoShort = "<path transform=\"scale(0.0234375)\" d=\"M885.78 117.88c4.68 11.09 7.74 64.1 7.47 76.84-.99 33.13-8.67 65.72-22.57 95.81-29.66 64.3-89.26 111.49-158.25 126.38-16.6 3.59-33.61 5.46-49.63 11.21-23.93 8.59-46.89 29.17-57.36 52.45 57.36 2.5 101.38-.18 150.43-34.71 7.8-5.49 14.49-12.31 21.6-18.64-1.94 6.32-4.68 26.3-5.91 33.82-4.05 24.94-8.5 46.26-18.37 69.66-18.84 44.63-53.04 81.03-96.4 102.62-29.77 14.85-52.62 14.9-72.84 47.7-8.25 13.35-12.99 28.57-13.8 44.24-.42 8.97.05 18.41-1.02 27.29-10.65 88.38-85.09 149.87-171 156.11 5.6-19.69 12.03-39.32 18.17-58.87l23.32-74.85 16.48-53.18c6.15-20.19 12.16-46.15 23.72-63.14 19.78-29.1 50.95-39.77 84.85-37.7 24.89 1.52 50.53-7.56 70.21-22.93 23.17-18.34 38.09-45.15 41.46-74.51-52.05-.36-106.06 1.14-157.74-.18 8.09-23.82 15.07-49.1 22.69-73.08 9.19-28.93 10.92-45.67 36.03-66.13 34.45-28.09 73.39-14.22 113.67-18.68 17.38-2.07 34.16-7.63 49.35-16.34 23.85-13.73 42.98-35.8 53.86-61.12 3.05-7.1 5.08-15.08 7.09-22.55-6.86-.67-17.67-.2-24.83-.1-12.1.17-24.2.21-36.3.13-39.15-.38-78.29-.5-117.44-.37-24.88-.01-63.17-1.54-86.31 3.62-9.07 2.21-17.86 5.46-26.19 9.69-22.85 11.72-41.56 30.16-53.6 52.84-8.45 16.23-15.74 43.55-21.43 61.95l-27.85 88.77L281.31 880.94c-21.58-12.77-45.64-35.75-59.94-56.24-32.35-45.6-45.11-102.25-35.43-157.32 6.41-36.18 22.64-69.91 46.9-97.5 6.41-7.34 14.21-15.35 22.15-20.91-7.2 31.33 1.25 64.93 16.64 92.49 23-65.6 15.23-155.61-46.58-197.65-2.29-1.57-4.69-3.09-7.04-4.59 4.55-.01 9.11.04 13.66.15 44.95 1.17 86.43 11.94 118.58 45.77-8.29-17.11-13.81-36.41-16.84-55.16-9.93-59.37 4.18-120.25 39.22-169.2 33.13-46.24 83.29-77.4 139.42-86.61-2.97 5.08-7.18 10.49-10.35 15.65-8.33 13.56-18.45 33.4-23.01 48.42 23.59-32.55 48.94-59.1 84.41-78.14 42.34-22.73 88.76-31.91 136.15-22.81 9.99 1.92 19.66 3.49 29.6 6.5-4.41 1.62-9.16 3.37-13.48 5.22-28.84 12.23-51.66 35.38-63.47 64.4 3.99-.49 8.77-.15 12.84-.13 7.94.04 15.96.11 23.89.14 76.46.25 150.4-34.3 197.14-95.14Z\"/>";

        /// <summary>The Flare brand mark as a ready <see cref="FlareSvgIcon"/>.</summary>
        public static FlareSvgIcon FlareLogo { get; } = new() { Data = FlareLogoShort };
    }
}
