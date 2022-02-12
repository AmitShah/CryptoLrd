mergeInto(LibraryManager.library, {
  OnHit: function (pos) {
    window.dispatchReactUnityEvent(
      "OnHit",
      Pointer_stringify(pos)
    );
  },
});