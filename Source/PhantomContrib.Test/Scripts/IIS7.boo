testSiteName = "newTestSite"
testApplicationPoolName = "testApplicationPoolName"

target createSiteWithAppPool:
    iis7_create_site(testSiteName, testApplicationPoolName, ".", 16161)
    print testSiteName + " created"
    
target removeSiteAndNotAppPool:
    iis7_remove_site(testSiteName)
    print testSiteName + " removed"
    
target removeSiteAndAppPool:
    iis7_remove_site(testSiteName, true)
    print testSiteName +  " and application pool removed"
    
target siteExists:
    if iis7_site_exists(testSiteName):
        print "site exist"
    else:
        print "site don't exist"