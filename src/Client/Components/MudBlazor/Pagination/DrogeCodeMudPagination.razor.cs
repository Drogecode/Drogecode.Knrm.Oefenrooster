// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Internal;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class DrogeCodeMudPagination : MudComponentBase
    {
        #region Css Classes

        private string Classname =>
            new CssBuilder("mud-pagination")
                .AddClass($"mud-pagination-{Variant.ToDescriptionString()}")
                .AddClass($"mud-pagination-{Size.ToDescriptionString()}")
                .AddClass("mud-pagination-disable-elevation", DisableElevation)
                .AddClass("mud-pagination-rtl", RightToLeft)
                .AddClass(Class)
                .Build();

        private string ItemClassname =>
            new CssBuilder("mud-pagination-item")
                .AddClass("mud-pagination-item-rectangular", Rectangular)
                .Build();

        private string SelectedItemClassname =>
            new CssBuilder(ItemClassname)
                .AddClass("mud-pagination-item-selected")
                .Build();

        #endregion

        #region Parameter

        private int _count = 1;

        private int _boundaryCount = 2;

        /// <summary>
        /// The number of items at the start and end of the pagination.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public int BoundaryCount
        {
            get => _boundaryCount;
            set
            {
                _boundaryCount = Math.Max(1, value);
            }
        }

        private int _middleCount = 3;

        /// <summary>
        /// The number of items in the middle of the pagination.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public int MiddleCount
        {
            get => _middleCount;
            set
            {
                _middleCount = Math.Max(1, value);
            }
        }

        private bool _selectedFirstSet;
        private DateOnly _selected;

        /// <summary>
        /// The selected page number.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public DateOnly Selected
        {
            get => _selected;
            set
            {
                if (_selected == value)
                    return;

                _selected = value;

                SelectedChanged.InvokeAsync(_selected);
            }
        }

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// If true, the pagination buttons are displayed rectangular.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public bool Rectangular { get; set; }

        /// <summary>
        /// The size of the component..
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// If true, no drop-shadow will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public bool DisableElevation { get; set; }

        /// <summary>
        /// If true, the pagination will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// If true, the navigate-to-previous-page button is shown.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowPreviousButton { get; set; } = true;

        /// <summary>
        /// If true, the navigate-to-next-page button is shown.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowNextButton { get; set; } = true;

        /// <summary>
        /// Invokes the callback when a control button is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<Page> ControlButtonClicked { get; set; }

        /// <summary>
        /// Invokes the callback when selected page changes.
        /// </summary>
        [Parameter]
        public EventCallback<DateOnly> SelectedChanged { get; set; }

        /// <summary>
        /// Custom before icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public string BeforeIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

        /// <summary>
        /// Custom next icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public string NextIcon { get; set; } = Icons.Material.Filled.NavigateNext;

        [CascadingParameter(Name = "RightToLeft")] public bool RightToLeft { get; set; }

        #endregion

        #region Methods

        private IEnumerable<PageObject> GeneratePagination()
        {
            var options = 2;
            var pages = new PageObject[options * 2 + 1];
            var date = Selected.AddMonths(-options);
            for (var i = 0; i < options * 2 + 1; i++)
            {
                pages[i] = new PageObject
                {
                    Name = DateTime.Today.Year == date.Year ? date.ToString("MMMM") : date.ToString("MMMM yyyy"),
                    Value = date
                };
                date = date.AddMonths(1);
            }
            return pages;
        }

        //triggered when the user clicks on a control button, e.g. the navigate-to-next-page-button
        private void OnClickControlButton(Page page)
        {
            ControlButtonClicked.InvokeAsync(page);
            NavigateTo(page);
        }

        //Last line cannot be tested because Page enum has 4 items
        /// <summary>
        /// Navigates to the specified page.
        /// </summary>
        /// <param name="page">The target page. page=Page.Next navigates to the next page.</param>
        [ExcludeFromCodeCoverage]
        public void NavigateTo(Page page)
        {
            switch (page)
            {
                case Page.Next:
                    Selected = Selected.AddMonths(1);
                    break;
                case Page.Previous:
                    Selected = Selected.AddMonths(-1);
                    break;
            }
        }
        #endregion

        private class PageObject
        {
            internal string Name { get; set; }
            internal DateOnly Value { get; set; }
        }
    }
}
