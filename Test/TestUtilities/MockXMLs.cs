namespace AlpimiTest.TestUtilities
{
    public static class MockXMLs
    {
        public static string GetBadDataXML()
        {
            return @"<?xml version=""1.0""?>
<?mso-application progid=""Excel.Sheet""?>
<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:o=""urn:schemas-microsoft-com:office:office""
 xmlns:x=""urn:schemas-microsoft-com:office:excel""
 xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:html=""http://www.w3.org/TR/REC-html40"">
 <DocumentProperties xmlns=""urn:schemas-microsoft-com:office:office"">
  <Version>16.00</Version>
 </DocumentProperties>
 <OfficeDocumentSettings xmlns=""urn:schemas-microsoft-com:office:office"">
  <AllowPNG/>
 </OfficeDocumentSettings>
 <ExcelWorkbook xmlns=""urn:schemas-microsoft-com:office:excel"">
  <WindowHeight>12225</WindowHeight>
  <WindowWidth>28800</WindowWidth>
  <WindowTopX>32767</WindowTopX>
  <WindowTopY>32767</WindowTopY>
  <ActiveSheet>10</ActiveSheet>
  <FirstVisibleSheet>1</FirstVisibleSheet>
  <ProtectStructure>False</ProtectStructure>
  <ProtectWindows>False</ProtectWindows>
  <WindowHeight xmlns="""">10635</WindowHeight>
  <WindowWidth xmlns="""">25005</WindowWidth>
  <WindowTopX xmlns="""">32767</WindowTopX>
  <WindowTopY xmlns="""">32767</WindowTopY>
  <ProtectStructure xmlns="""">False</ProtectStructure>
  <ProtectWindows xmlns="""">False</ProtectWindows>
 </ExcelWorkbook>
 <Styles>
  <Style ss:ID=""Default"" ss:Name=""Normal"">
   <Alignment ss:Vertical=""Bottom""/>
   <Borders/>
   <Font ss:FontName=""Arial"" x:CharSet=""238"" x:Family=""Swiss"" ss:Size=""11""
    ss:Color=""#000000""/>
   <Interior/>
   <NumberFormat/>
   <Protection/>
  </Style>
  <Style ss:ID=""s62"">
   <Alignment ss:Horizontal=""Left"" ss:Vertical=""Center""/>
   <Font ss:FontName=""Arial"" x:CharSet=""238"" x:Family=""Swiss"" ss:Size=""11""
    ss:Color=""#FFFFFF"" ss:Bold=""1""/>
   <Interior ss:Color=""#70AD47"" ss:Pattern=""Solid""/>
  </Style>
  <Style ss:ID=""s63"">
   <Font ss:FontName=""Arial"" x:CharSet=""238"" x:Family=""Swiss"" ss:Size=""11""
    ss:Color=""#000000""/>
   <NumberFormat ss:Format=""hh:mm:ss""/>
  </Style>
 </Styles>
 <Worksheet ss:Name=""Lesson periods"">
  <Table ss:ExpandedColumnCount=""1"" ss:ExpandedRowCount=""5"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""295.5""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson block start hour (HH:MM:SS)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell ss:StyleID=""s63""><Data ss:Type=""String"">bad</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell ss:StyleID=""s63""/>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell ss:StyleID=""s63""/>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell ss:StyleID=""s63""/>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Days off"">
  <Table ss:ExpandedColumnCount=""3"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""171""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""188.25""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""187.5""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Day off name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Start date (MM/DD/YYYY)</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">End date (MM/DD/YYYY)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">Christmas</Data></Cell>
    <Cell><Data ss:Type=""String"">bad</Data></Cell>
    <Cell><Data ss:Type=""String"">bad</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>2</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Classroom types"">
  <Table ss:ExpandedColumnCount=""1"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""155.25""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Classroom type name</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">Lab</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>4</ActiveRow>
     <RangeSelection>R3C1:R5C1</RangeSelection>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Lesson types"">
  <Table ss:ExpandedColumnCount=""2"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""138""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""200.25""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson type name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Color (number in the range 1 - 359)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">Cab</Data></Cell>
    <Cell><Data ss:Type=""String"">bad</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>1</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Teachers"">
  <Table ss:ExpandedColumnCount=""3"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""105""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""87""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""96.75""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Email</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Surname</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">jacekp@pk.edu.pl</Data></Cell>
    <Cell><Data ss:Type=""String"">Jacek</Data></Cell>
    <Cell><Data ss:Type=""String"">Piet</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>12</ActiveRow>
     <ActiveCol>1</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Groups"">
  <Table ss:ExpandedColumnCount=""2"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""84.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""97.5""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Group name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Student count</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">14K4</Data></Cell>
    <Cell><Data ss:Type=""String"">bad</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>1</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Subgroups"">
  <Table ss:ExpandedColumnCount=""3"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""112.5""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""93.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""87.75""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Subgroup name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Student count</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Group name</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">K04</Data></Cell>
    <Cell><Data ss:Type=""String"">bad</Data></Cell>
    <Cell><Data ss:Type=""String"">14K2</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>1</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Classrooms"">
  <Table ss:ExpandedColumnCount=""3"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""106.5""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""75.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""249.75""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Classroom name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Capacity</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Classroom types (separated by semicolon)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">G123</Data></Cell>
    <Cell><Data ss:Type=""String"">bad</Data></Cell>
    <Cell><Data ss:Type=""String"">Ball</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>1</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Lessons"">
  <Table ss:ExpandedColumnCount=""6"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""177.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""87""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""185.25""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""206.25""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""249.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""399.75""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson hours</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson type</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Teacher's email</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Classroom types (separated by semicolon)</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Subgroups (seperated by semicolon, format: parent group/subgroup)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">Math</Data></Cell>
    <Cell><Data ss:Type=""String"">bad</Data></Cell>
    <Cell><Data ss:Type=""String"">Lab</Data></Cell>
    <Cell><Data ss:Type=""String"">jacekp@pk.edu.pl</Data></Cell>
    <Cell><Data ss:Type=""String"">Hall</Data></Cell>
    <Cell><Data ss:Type=""String"">14K2/K03</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>1</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Teacher's availability"">
  <Table ss:ExpandedColumnCount=""4"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""128.25""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""117.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""129.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""120""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Teacher's email</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Day of week</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson number from</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson number to</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">jacekp@pk.edu.pl</Data></Cell>
    <Cell><Data ss:Type=""String"">bad</Data></Cell>
    <Cell><Data ss:Type=""String"">bad</Data></Cell>
    <Cell><Data ss:Type=""String"">bad</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>3</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Students"">
  <Table ss:ExpandedColumnCount=""3"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""99.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""110.25""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""270""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Album number</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Student's group</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Student's subgroups (separated by semicolon)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">D/1236</Data></Cell>
    <Cell><Data ss:Type=""String"">14K3</Data></Cell>
    <Cell><Data ss:Type=""String"">K01; K03</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Selected/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>11</ActiveRow>
     <ActiveCol>2</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
</Workbook>
";
        }

        public static string GetBadReferencesXML()
        {
            return @"<?xml version=""1.0""?>
<?mso-application progid=""Excel.Sheet""?>
<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:o=""urn:schemas-microsoft-com:office:office""
 xmlns:x=""urn:schemas-microsoft-com:office:excel""
 xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:html=""http://www.w3.org/TR/REC-html40"">
 <DocumentProperties xmlns=""urn:schemas-microsoft-com:office:office"">
  <Version>16.00</Version>
 </DocumentProperties>
 <OfficeDocumentSettings xmlns=""urn:schemas-microsoft-com:office:office"">
  <AllowPNG/>
 </OfficeDocumentSettings>
 <ExcelWorkbook xmlns=""urn:schemas-microsoft-com:office:excel"">
  <WindowHeight>7530</WindowHeight>
  <WindowWidth>21570</WindowWidth>
  <WindowTopX>32767</WindowTopX>
  <WindowTopY>32767</WindowTopY>
  <ActiveSheet>10</ActiveSheet>
  <FirstVisibleSheet>1</FirstVisibleSheet>
  <ProtectStructure>False</ProtectStructure>
  <ProtectWindows>False</ProtectWindows>
  <WindowHeight xmlns="""">10635</WindowHeight>
  <WindowWidth xmlns="""">25005</WindowWidth>
  <WindowTopX xmlns="""">32767</WindowTopX>
  <WindowTopY xmlns="""">32767</WindowTopY>
  <ProtectStructure xmlns="""">False</ProtectStructure>
  <ProtectWindows xmlns="""">False</ProtectWindows>
 </ExcelWorkbook>
 <Styles>
  <Style ss:ID=""Default"" ss:Name=""Normal"">
   <Alignment ss:Vertical=""Bottom""/>
   <Borders/>
   <Font ss:FontName=""Arial"" x:CharSet=""238"" x:Family=""Swiss"" ss:Size=""11""
    ss:Color=""#000000""/>
   <Interior/>
   <NumberFormat/>
   <Protection/>
  </Style>
  <Style ss:ID=""s62"">
   <Alignment ss:Horizontal=""Left"" ss:Vertical=""Center""/>
   <Font ss:FontName=""Arial"" x:CharSet=""238"" x:Family=""Swiss"" ss:Size=""11""
    ss:Color=""#FFFFFF"" ss:Bold=""1""/>
   <Interior ss:Color=""#70AD47"" ss:Pattern=""Solid""/>
  </Style>
  <Style ss:ID=""s63"">
   <Font ss:FontName=""Arial"" x:CharSet=""238"" x:Family=""Swiss"" ss:Size=""11""
    ss:Color=""#000000""/>
   <NumberFormat ss:Format=""hh:mm:ss""/>
  </Style>
 </Styles>
 <Worksheet ss:Name=""Lesson periods"">
  <Table ss:ExpandedColumnCount=""1"" ss:ExpandedRowCount=""5"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""295.5""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson block start hour (HH:MM:SS)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell ss:StyleID=""s63""><Data ss:Type=""DateTime"">1899-12-31T12:00:00.000</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell ss:StyleID=""s63""/>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell ss:StyleID=""s63""/>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell ss:StyleID=""s63""/>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>4</ActiveRow>
     <RangeSelection>R3C1:R5C1</RangeSelection>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Days off"">
  <Table ss:ExpandedColumnCount=""3"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""171""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""188.25""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""187.5""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Day off name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Start date (MM/DD/YYYY)</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">End date (MM/DD/YYYY)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">Christmas</Data></Cell>
    <Cell><Data ss:Type=""String"">12/24/2025</Data></Cell>
    <Cell><Data ss:Type=""String"">12/25/2025</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>4</ActiveRow>
     <RangeSelection>R3C1:R5C3</RangeSelection>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Classroom types"">
  <Table ss:ExpandedColumnCount=""1"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""155.25""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Classroom type name</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">Lab</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>4</ActiveRow>
     <RangeSelection>R3C1:R5C1</RangeSelection>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Lesson types"">
  <Table ss:ExpandedColumnCount=""2"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""138""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""200.25""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson type name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Color (number in the range 1 - 359)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">Cab</Data></Cell>
    <Cell><Data ss:Type=""Number"">121</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>4</ActiveRow>
     <RangeSelection>R3C1:R5C2</RangeSelection>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Teachers"">
  <Table ss:ExpandedColumnCount=""3"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""105""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""87""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""96.75""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Email</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Surname</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">jacekp@pk.edu.pl</Data></Cell>
    <Cell><Data ss:Type=""String"">Jacek</Data></Cell>
    <Cell><Data ss:Type=""String"">Piet</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>4</ActiveRow>
     <RangeSelection>R3C1:R5C3</RangeSelection>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Groups"">
  <Table ss:ExpandedColumnCount=""2"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""84.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""97.5""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Group name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Student count</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">14K4</Data></Cell>
    <Cell><Data ss:Type=""Number"">60</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>4</ActiveRow>
     <RangeSelection>R3C1:R5C2</RangeSelection>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Subgroups"">
  <Table ss:ExpandedColumnCount=""3"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""112.5""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""93.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""87.75""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Subgroup name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Student count</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Group name</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">K04</Data></Cell>
    <Cell><Data ss:Type=""Number"">20</Data></Cell>
    <Cell><Data ss:Type=""String"">badRef</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>2</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Classrooms"">
  <Table ss:ExpandedColumnCount=""3"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""106.5""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""75.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""249.75""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Classroom name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Capacity</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Classroom types (separated by semicolon)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">G123</Data></Cell>
    <Cell><Data ss:Type=""Number"">60</Data></Cell>
    <Cell><Data ss:Type=""String"">badRef</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>2</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Lessons"">
  <Table ss:ExpandedColumnCount=""6"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""177.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""87""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""185.25""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""206.25""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""249.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""399.75""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson name</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson hours</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson type</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Teacher's email</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Classroom types (separated by semicolon)</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Subgroups (seperated by semicolon, format: parent group/subgroup)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">Math</Data></Cell>
    <Cell><Data ss:Type=""Number"">30</Data></Cell>
    <Cell><Data ss:Type=""String"">badRef</Data></Cell>
    <Cell><Data ss:Type=""String"">badRef</Data></Cell>
    <Cell><Data ss:Type=""String"">badRef</Data></Cell>
    <Cell><Data ss:Type=""String"">badRef</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>5</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Teacher's availability"">
  <Table ss:ExpandedColumnCount=""4"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""128.25""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""117.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""129.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""120""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Teacher's email</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Day of week</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson number from</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Lesson number to</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">badRef</Data></Cell>
    <Cell><Data ss:Type=""String"">Sunday</Data></Cell>
    <Cell><Data ss:Type=""Number"">1</Data></Cell>
    <Cell><Data ss:Type=""Number"">2</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
 <Worksheet ss:Name=""Students"">
  <Table ss:ExpandedColumnCount=""3"" ss:ExpandedRowCount=""2"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""14.25"">
   <Column ss:AutoFitWidth=""0"" ss:Width=""99.75""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""110.25""/>
   <Column ss:AutoFitWidth=""0"" ss:Width=""270""/>
   <Row ss:AutoFitHeight=""0"" ss:Height=""24"">
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Album number</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Student's group</Data></Cell>
    <Cell ss:StyleID=""s62""><Data ss:Type=""String"">Student's subgroups (separated by semicolon)</Data></Cell>
   </Row>
   <Row ss:AutoFitHeight=""0"">
    <Cell><Data ss:Type=""String"">D/1236</Data></Cell>
    <Cell><Data ss:Type=""String"">badRef</Data></Cell>
    <Cell><Data ss:Type=""String"">badRef</Data></Cell>
   </Row>
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <Unsynced/>
   <Selected/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
     <ActiveCol>2</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
</Workbook>

";
        }

        public static string GetCorrectXML()
        {
            return @"<?xml version=""1.0""?>
<?mso-application progid=""Excel.Sheet""?>
<ss:Workbook xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:x=""urn:schemas-microsoft-com:office:excel"" xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet"" xmlns:html=""http://www.w3.org/TR/REC-html40"">
  <o:DocumentProperties>
    <Author>Alpimi</Author>
    <LastAuthor>Export</LastAuthor>
    <Created>2025-10-30T19:21:17Z</Created>
    <LastSaved>2025-10-30T19:21:17Z</LastSaved>
    <Version>16.00</Version>
  </o:DocumentProperties>
  <x:ExcelWorkbook>
    <WindowHeight>10635</WindowHeight>
    <WindowWidth>25005</WindowWidth>
    <WindowTopX>32767</WindowTopX>
    <WindowTopY>32767</WindowTopY>
    <ProtectStructure>False</ProtectStructure>
    <ProtectWindows>False</ProtectWindows>
  </x:ExcelWorkbook>
  <ss:Styles>
    <ss:Style ss:ID=""Default"" ss:Name=""Normal"">
      <ss:Alignment ss:Vertical=""Bottom"" />
      <ss:Borders />
      <ss:Font ss:FontName=""Arial"" x:CharSet=""238"" x:Family=""Swiss"" ss:Size=""11"" ss:Color=""#000000"" />
      <ss:Interior />
      <ss:NumberFormat />
      <ss:Protection />
    </ss:Style>
    <ss:Style ss:ID=""s18"">
      <ss:Alignment ss:Horizontal=""Left"" ss:Vertical=""Center"" />
      <ss:Font ss:FontName=""Arial"" x:CharSet=""238"" x:Family=""Swiss"" ss:Size=""11"" ss:Color=""#FFFFFF"" ss:Bold=""1"" />
      <ss:Interior ss:Color=""#70AD47"" ss:Pattern=""Solid"" />
    </ss:Style>
    <ss:Style ss:ID=""s19"">
      <ss:Font ss:FontName=""Arial"" x:CharSet=""238"" x:Family=""Swiss"" ss:Size=""11"" ss:Color=""#000000"" />
      <ss:NumberFormat ss:Format=""hh:mm:ss"" />
    </ss:Style>
  </ss:Styles>
  <ss:Worksheet ss:Name=""Lesson periods"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""295.5"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Lesson block start hour (HH:MM:SS)</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell ss:StyleID=""s19"">
          <ss:Data ss:Type=""DateTime"">1899-12-31T12:00:00.000</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell ss:StyleID=""s19"">
          <ss:Data ss:Type=""DateTime"">1899-12-31T13:00:00.000</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell ss:StyleID=""s19"">
          <ss:Data ss:Type=""DateTime"">1899-12-31T14:00:00.000</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell ss:StyleID=""s19"">
          <ss:Data ss:Type=""DateTime"">1899-12-31T15:00:00.000</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
  <ss:Worksheet ss:Name=""Days off"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""171"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""188.25"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""187.5"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Day off name</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Start date (MM/DD/YYYY)</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">End date (MM/DD/YYYY)</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Christmas</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">12/24/2025</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">12/25/2025</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Christmas2</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">12/25/2025</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">12/26/2025</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Christmas4</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">12/27/2025</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">12/28/2025</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Christmas3</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">12/26/2025</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">12/27/2025</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
  <ss:Worksheet ss:Name=""Classroom types"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""155.25"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Classroom type name</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Lab</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Hall</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Call</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Ball</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
  <ss:Worksheet ss:Name=""Lesson types"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""138"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""200"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Lesson type name</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Color (number in the range 1 - 359)</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Cab</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">121</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Bab</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">212</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Lab</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">120</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Pab</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">122</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
  <ss:Worksheet ss:Name=""Teachers"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""105"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""87"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""96.75"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Email</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Name</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Surname</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Jacek</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Piet</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp3@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Jacek</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Piet2</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp1@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Jacek2</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Piet4</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp2@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Jacek1</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Piet3</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
  <ss:Worksheet ss:Name=""Groups"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""84.75"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""97.5"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Group name</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Student count</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K4</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">60</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K5</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">70</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K2</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">40</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K3</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">50</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
  <ss:Worksheet ss:Name=""Subgroups"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""112.5"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""93.75"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""87.75"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Subgroup name</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Student count</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Group name</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">K04</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">20</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K2</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">K03</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">20</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K2</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">K01</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">20</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K3</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">K03</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">20</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K3</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
  <ss:Worksheet ss:Name=""Classrooms"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""106.5"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""75.75"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""250"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Classroom name</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Capacity</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Classroom types (separated by semicolon)</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">G123</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">60</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Ball</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">G122</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">50</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Call</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">G120</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">30</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Hall</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">G121</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">40</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Ball</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
  <ss:Worksheet ss:Name=""Lessons"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""177.75"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""87"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""185.25"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""206.25"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""250"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""400"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Lesson name</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Lesson hours</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Lesson type</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Teacher's email</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Classroom types (separated by semicolon)</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Subgroups (seperated by semicolon, format: parent group/subgroup)</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Math</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">30</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Lab</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Hall</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K2/K03</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Wrath</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">40</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Bab</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp1@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Lab; Call</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K2/K04; 14K2/K03</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Hatred</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">50</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Bab</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp1@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Lab</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K2/K03; 14K3/K01; 14K3/K03</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Discipline</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">60</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Pab</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Lab; Call; Ball</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K2/K03</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
  <ss:Worksheet ss:Name=""Teacher's availability"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""128.25"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""117.75"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""130"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""120"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Teacher's email</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Day of week</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Lesson number from</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Lesson number to</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Sunday</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">1</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">2</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Thursday</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">1</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">1</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">jacekp@pk.edu.pl</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">Monday</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">1</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""Number"">1</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
  <ss:Worksheet ss:Name=""Students"">
    <ss:Table ss:DefaultRowHeight=""14.25"">
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""99.75"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""110"" />
      <ss:Column ss:AutoFitWidth=""0"" ss:Width=""270"" />
      <ss:Row ss:AutoFitHeight=""0"" ss:Height=""24"">
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Album number</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Student's group</ss:Data>
        </ss:Cell>
        <ss:Cell ss:StyleID=""s18"">
          <ss:Data ss:Type=""String"">Student's subgroups (separated by semicolon)</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">D/1236</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K3</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">K01; K03</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">D/1237</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K3</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">K03</ss:Data>
        </ss:Cell>
      </ss:Row>
      <ss:Row>
        <ss:Cell>
          <ss:Data ss:Type=""String"">D/1235</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">14K2</ss:Data>
        </ss:Cell>
        <ss:Cell>
          <ss:Data ss:Type=""String"">K04</ss:Data>
        </ss:Cell>
      </ss:Row>
    </ss:Table>
  </ss:Worksheet>
</ss:Workbook>";
        }
    }
}
