
mergeInto(LibraryManager.library, {

  SetCursor: function (str) {
    var canvases = document.getElementsByTagName("canvas");
    if (canvases.length > 0)
    {
      canvases.item(0).style.cursor = Pointer_stringify(str);
    }
  }
});