// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace TigerApp.iOS.Pages.StoreLocator
{
    [Register ("StoreLocatorViewController")]
    partial class StoreLocatorViewController
    {
        [Outlet]
        UIKit.UIActivityIndicatorView activityIndicator { get; set; }


        [Outlet]
        UIKit.UIButton chiamaButton { get; set; }


        [Outlet]
        UIKit.UILabel empyStoreListLbl { get; set; }


        [Outlet]
        UIKit.UILabel infoCityLabel { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint infoCollectionViewHeightConstraint { get; set; }


        [Outlet]
        UIKit.UILabel infoDistanceLabel { get; set; }


        [Outlet]
        UIKit.UILabel infoStreetLabel { get; set; }


        [Outlet]
        UIKit.UIView InfoSubview { get; set; }


        [Outlet]
        UIKit.UILabel infoTelephone { get; set; }


        [Outlet]
        UIKit.UIView infoView { get; set; }


        [Outlet]
        UIKit.UILabel infoWorkhours { get; set; }


        [Outlet]
        MapKit.MKMapView mapView { get; set; }


        [Outlet]
        UIKit.UIView phoneSeparatorView { get; set; }


        [Outlet]
        UIKit.UITextField searchField { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint storeCollectionViewHeightConstraint { get; set; }


        [Outlet]
        UIKit.UICollectionView storesCollectionView { get; set; }


        [Outlet]
        UIKit.UIButton userLocationBtn { get; set; }


        [Action ("OnBackButtonClick:")]
        partial void OnBackButtonClick (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (activityIndicator != null) {
                activityIndicator.Dispose ();
                activityIndicator = null;
            }

            if (chiamaButton != null) {
                chiamaButton.Dispose ();
                chiamaButton = null;
            }

            if (empyStoreListLbl != null) {
                empyStoreListLbl.Dispose ();
                empyStoreListLbl = null;
            }

            if (infoCityLabel != null) {
                infoCityLabel.Dispose ();
                infoCityLabel = null;
            }

            if (infoCollectionViewHeightConstraint != null) {
                infoCollectionViewHeightConstraint.Dispose ();
                infoCollectionViewHeightConstraint = null;
            }

            if (infoDistanceLabel != null) {
                infoDistanceLabel.Dispose ();
                infoDistanceLabel = null;
            }

            if (infoStreetLabel != null) {
                infoStreetLabel.Dispose ();
                infoStreetLabel = null;
            }

            if (InfoSubview != null) {
                InfoSubview.Dispose ();
                InfoSubview = null;
            }

            if (infoTelephone != null) {
                infoTelephone.Dispose ();
                infoTelephone = null;
            }

            if (infoView != null) {
                infoView.Dispose ();
                infoView = null;
            }

            if (infoWorkhours != null) {
                infoWorkhours.Dispose ();
                infoWorkhours = null;
            }

            if (mapView != null) {
                mapView.Dispose ();
                mapView = null;
            }

            if (phoneSeparatorView != null) {
                phoneSeparatorView.Dispose ();
                phoneSeparatorView = null;
            }

            if (searchField != null) {
                searchField.Dispose ();
                searchField = null;
            }

            if (storeCollectionViewHeightConstraint != null) {
                storeCollectionViewHeightConstraint.Dispose ();
                storeCollectionViewHeightConstraint = null;
            }

            if (storesCollectionView != null) {
                storesCollectionView.Dispose ();
                storesCollectionView = null;
            }

            if (userLocationBtn != null) {
                userLocationBtn.Dispose ();
                userLocationBtn = null;
            }
        }
    }
}