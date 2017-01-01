# 1.2.1

* __Application Measurement__
  + If you do not wish to enable measurement for an application instance, you
  can call `[[GMRConfiguration sharedInstance] setIsEnabled:NO]` anytime. Call
  `[[GMRConfiguration sharedInstance] setIsEnabled:YES]` to reenable
  measurement. These calls override any value of IS_MEASUREMENT_ENABLED in
  GoogleService-Info.plist.
* Performance improvements
* Bug fixes

# 1.1.1

* __Application Measurement__
  + This release provides features for collecting anonymous statistics on
    application events such as when a user first opens your application. This
    data is collected only if your project contains a GoogleService-Info.plist
    with a valid `GOOGLE_APP_ID`. If you do not wish to have this data reported,
    add the key `IS_MEASUREMENT_ENABLED = NO` to your project's
    GoogleService-Info.plist before releasing your application or its next
    update.

# 1.1.0

* Performance improvements
* Bug fixes

# 1.0.8

* Performance improvements
* Bug fixes

# 1.0.7

* Performance improvements
* Bug fixes
