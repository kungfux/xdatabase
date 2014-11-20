:: Copyright 2010-2014 Fuks Alexander. Contacts: kungfux2010@gmail.com
::
:: Licensed under the Apache License, Version 2.0 (the "License");
:: you may not use this file except in compliance with the License.
:: You may obtain a copy of the License at
:: 
::   http://www.apache.org/licenses/LICENSE-2.0
::
:: Unless required by applicable law or agreed to in writing, software
:: distributed under the License is distributed on an "AS IS" BASIS,
:: WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
:: See the License for the specific language governing permissions and
:: limitations under the License.


:: This cmd perform test of code coverage by unit tests and then build html reports
:: It is needed to install OpenCover, ReportGenerator and NUnit testing framework
::   https://github.com/OpenCover/opencover/
::   http://reportgenerator.codeplex.com/
::   http://nunit.org/

@echo off
:: OpenCover location
set OpenCoverConsole=%LOCALAPPDATA%\Apps\OpenCover\OpenCover.Console.exe
if not exist "%OpenCoverConsole%" echo "OpenCover is not found. Please install or check its location." && goto :eof
:: ReportGenerator location
set ReportGenerator=%LOCALAPPDATA%\Apps\ReportGenerator\bin\ReportGenerator.exe
if not exist "%ReportGenerator%" echo "ReportGenerator is not found. Please install or check its location." && goto :eof
:: NUnit location
set NUnitConsole=C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console-x86.exe
if not exist "%NUnitConsole%" echo "NUnitConsole is not found. Please install or check its location." && goto :eof

:: Perform code coverage test
%OpenCoverConsole% "-register:user" "-target:%NUnitConsole%" "-targetdir:..\XclassTests\bin\Release" "-targetargs:XclassTests.dll /noshadow" "-output:.\TestsCodeCoverageResults.xml"
:: Build reports
%ReportGenerator% "-reports:.\TestsCodeCoverageResults.xml" "-targetdir:." "-reporttypes:html"
:: Open report
@start %cd%\index.htm