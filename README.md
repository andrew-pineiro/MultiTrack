# MultiTrack
> Andrew Pineiro | 10/20/2023
---

## Summary

This will allow the input of any UPS, FedEx, USPS tracking numbers and will redirect them to the correct URL.


## Logic

#### UPS

To satisfy the logic for a UPS tracking number it needs to meet any of the following:

* Start with “1Z”

* 9 Characters in Length

* Starts with “K”

* Starts with “H”

#### FedEx

To meet the logic for a FedEx tracking number, it needs to contain ALL of the following:

* Tracking ID needs to be ALL numbers

* 10,12,15,20,22, or 34 characters in length

* Starts with 27, 96, 39, 64, 56, or 78

#### USPS

Meeting the USPS tracking number logic requires the following:

* Tracking ID needs to be ALL numbers

* 22 characters in length

* Start with “94” or end with “US”

#### Other

If none of the above requirements are met, it will redirect to the root directory of the application (/) and will output the following message:

* “Tracking Number Not Found.”

## Logging

The API will log basic startup/shutdown messages and critical errors to the application. The log path (below) used by the application is stored in the appsettings.json file, which is located where the application is running from.

Log file: `multitrack-log-{date}.txt`