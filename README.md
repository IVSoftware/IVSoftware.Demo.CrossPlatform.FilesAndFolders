In addition to Stephen's excellent (and genuinely recursive) treatment of this, I decided to leave my own answer up (albeit simplified) because backing a tree-style view with portable XML is still a pretty decent way of going about doing this. Basically, it means that the same business logic can be used more universally, e.g. Maui, WinForms, WPF etc. 

The latest NuGet package for [XBoundObject](https://www.nuget.org/packages/IVSoftware.Portable.Xml.Linq.XBoundObject/2.0.0) introduces the `IBoundObjectView` interface. It can be implemented from scratch of course, but the package offers an optional portable implementation for it, In this case, the `FileItem` data model inherits it ("is a" relationship) but it could also for example inherit `ObservableObject` and use the portable implementation in a "has a" relationship instead. We can allow the base class to provide the `Text` for the label and the `Space` property that creates the smoke-and-mirrors illusion of depth in the `CollectrionView`. The snippet below overrides the `PlusMinusGlyph`in order to supply custim images for the expander button.


```
class FileItem : XBoundViewObjectImplementer
{
    // Font family 'file-and-folder-icons' is a customized https://fontello.com font.
    public override string PlusMinusGlyph
    {
        get
        {
            switch (PlusMinus)
            {
                case PlusMinus.Collapsed:
                    return "\uE803";
                case PlusMinus.Partial:
                case PlusMinus.Expanded:
                    return "\uE804";
                default:
                    return "\uE805";
            }
        }
    }
}
```

___

#### Data Template

There is no need for recursion here. The width of the `BoxView` creates the 2-dimensional effect.

```
 <DataTemplate>
    <Grid ColumnDefinitions="Auto,40,*" RowDefinitions="40" >
        <BoxView 
            Grid.Column="0" 
            WidthRequest="{Binding Space}"
            BackgroundColor="{
                Binding BackgroundColor, 
                Source={x:Reference FileCollectionView}}"/>
        <Button 
            Grid.Column="1" 
            Text="{Binding PlusMinusGlyph}" 
            TextColor="Black"
            Command="{
                Binding PlusMinusToggleCommand, 
                Source={x:Reference MainPageViewModel}}"
            CommandParameter="{Binding .}"
            FontSize="16"
            FontFamily="file-and-folder-icons"
            BackgroundColor="Transparent"
            Padding="0"
            BorderWidth="0"
            VerticalOptions="Fill"
            HorizontalOptions="Fill"
            MinimumHeightRequest="0"
            MinimumWidthRequest="0"
            CornerRadius="0"/>
        <Label 
            Grid.Column="2"
            Text="{Binding Text}" 
            VerticalTextAlignment="Center" Padding="2,0,0,0"/>
    </Grid>
</DataTemplate>
```

___

**How it Works**

When the `MainPage` ctor calls the `Show` extension for the example path, the result will be a 2D projection onto the root `XElement` with attributes for visibility and the `+/-` expander. Alternatively, to add nodes to the XML hierarchy _without_ showing them, the `XRoot.FindOrCreate<T>(path)` method instead (see [full documentation](https://github.com/IVSoftware/IVSoftware.Portable.Xml.Linq.XBoundObject/blob/master/README/Placer.md) in the repo).

##### MainPage View

```
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        string path =
            @"C:\Github\IVSoftware\Demo\IVSoftware.Demo.CrossPlatform.FilesAndFolders\BasicPlacement.Maui\BasicPlacement.Maui.csproj"
            .Replace('\\', Path.DirectorySeparatorChar);

        BindingContext.XRoot.Show(path);
    }
    new MainPageViewModel BindingContext => (MainPageViewModel)base.BindingContext;
}
```

##### `XRoot` as Viewed in Debugger

```xml
<root viewcontext="[ViewContext]">
  <xnode datamodel="[FileItem]" text="C:" isvisible="True" plusminus="Expanded">
    <xnode datamodel="[FileItem]" text="Github" isvisible="True" plusminus="Expanded">
      <xnode datamodel="[FileItem]" text="IVSoftware" isvisible="True" plusminus="Expanded">
        <xnode datamodel="[FileItem]" text="Demo" isvisible="True" plusminus="Expanded">
          <xnode datamodel="[FileItem]" text="IVSoftware.Demo.CrossPlatform.FilesAndFolders" isvisible="True" plusminus="Expanded">
            <xnode datamodel="[FileItem]" text="BasicPlacement.Maui" isvisible="True" plusminus="Expanded">
              <xnode datamodel="[FileItem]" text="BasicPlacement.Maui.csproj" isvisible="True" />
            </xnode>
          </xnode>
        </xnode>
      </xnode>
    </xnode>
  </xnode>
</root>
```


##### Traversing the Hierarchy.
___

## In-Depth Example Code

This repo contains a functional file browser example that uses the same portable backend tree for Maui (tested for Android and Windows) and for WinForms.