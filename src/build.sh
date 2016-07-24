#	build.sh 
#	For	Windows 
#	... 
.paket/paket.exe restore 
exit_code=$? 
if [ $exit_code -ne 0 ]; then
exit $exit_code
fi 

packages/FAKE/tools/FAKE.exe	$@	--fsiargs	build.fsx 
