$files = Get-ChildItem installer\GDItemAssistant*.exe
foreach ($file in $files) {
	echo $file.Name
	if (test-path ("Installer\" + $file.Name + ".*")) {

	} else {
		echo 0 > ("installer\" + $file.Name + "." + ($(CertUtil -hashfile ("Installer\" + $file.Name) SHA256)[1] -replace " ",""));
	}
}