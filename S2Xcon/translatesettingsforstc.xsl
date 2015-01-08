<?xml version="1.0" encoding="UTF-8"?>
<!-- Translate Settings XSL file for Scan to Connect.  Remove all fields except comm info -->
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

    <xsl:variable name="PromptFields" select="/DevInfo/PromptDuringRestore"/>
    <xsl:variable name="lcletters">abcdefghijklmnopqrstuvwxyz</xsl:variable>
    <xsl:variable name="ucletters">ABCDEFGHIJKLMNOPQRSTUVWXYZ</xsl:variable>


  <!-- Find the path to and load the Comm Data Model -->
    <xsl:variable name="CommDataModelName1">
        <xsl:for-each select="/DevInfo/DataModels/File/@Path">
            <xsl:if test="contains(translate(.,$ucletters,$lcletters),'communicationsschema.xml')">
                <xsl:value-of select="."/>
                <xsl:value-of select="'*'"/>
            </xsl:if>
        </xsl:for-each>
    </xsl:variable> 
    <xsl:variable name="CommDataModelName" select="substring-before($CommDataModelName1,'*')"/>
    <xsl:variable name="CommDefaults" select="document($CommDataModelName)"/>

    <!-- Find the path to and load the Funk Data Model -->
    <xsl:variable name="FunkDataModelName1">
        <xsl:for-each select="/DevInfo/DataModels/File/@Path">
            <xsl:if test="contains(translate(.,$ucletters,$lcletters),'funksecuritydatamodel.xml')">
                <xsl:value-of select="."/>
                <xsl:value-of select="'*'"/>
            </xsl:if>
        </xsl:for-each>
    </xsl:variable> 
    <xsl:variable name="FunkDataModelName" select="substring-before($FunkDataModelName1,'*')"/>
    <xsl:variable name="FunkDefaults" select="document($FunkDataModelName)"/>

    <!-- Find the path to and load the IQueue Data Model -->
    <xsl:variable name="IQueueDataModelName1">
        <xsl:for-each select="/DevInfo/DataModels/File/@Path">
            <xsl:if test="contains(translate(.,$ucletters,$lcletters),'iqueueschema.xml')">
                <xsl:value-of select="."/>
                <xsl:value-of select="'*'"/>
            </xsl:if>
        </xsl:for-each>
    </xsl:variable> 
    <xsl:variable name="IQueueDataModelName" select="substring-before($IQueueDataModelName1,'*')"/>
    <xsl:variable name="IQueueDefaults" select="document($IQueueDataModelName)"/>
    <xsl:variable name="ZeroConfig" select="/DevInfo/Subsystem[@Name='Funk Security']/Group[@Name='802.11 Radio']/Field[@Name='ZeroConfig']"/>
    <xsl:variable name="ZeroConfig2" select="/DevInfo/Subsystem[@Name='Communications']/Group[@Name='802.11 Radio']/Field[@Name='ZeroConfig']"/>

    <!-- Find the path to and load the WWAN Data Model -->
    <xsl:variable name="WWANDataModelName1">
      <xsl:for-each select="/DevInfo/DataModels/File/@Path">
        <xsl:if test="contains(translate(.,$ucletters,$lcletters),'wwandatamodel.xml')">
          <xsl:value-of select="."/>
          <xsl:value-of select="'*'"/>
        </xsl:if>
      </xsl:for-each>
    </xsl:variable>

  <xsl:variable name="WWANDataModelName" select="substring-before($WWANDataModelName1,'*')"/>
  <xsl:variable name="WWANDefaults" select="document($WWANDataModelName)"/>

  <!-- Convert one settings file to another with some fields stripped out -->
<xsl:template match="/">
    <!--xsl:comment>Data Model = <xsl:value-of select="$CommDataModelName"/></xsl:comment-->
    <!--xsl:comment>Data Model = <xsl:value-of select="$FunkDataModelName"/></xsl:comment-->
    <!--xsl:comment>Data Model = <xsl:value-of select="$IQueueDataModelName"/></xsl:comment-->
	<xsl:apply-templates select="DevInfo"/>
</xsl:template>

<!-- Match the DevInfo -->
<xsl:template match="DevInfo">
    <xsl:comment>**************************</xsl:comment>
    <xsl:comment>**   XSL Version 1.01   **</xsl:comment>
    <xsl:comment>**************************</xsl:comment>
	<DevInfo>
		<!-- Bring over all attributes one for one -->
		<xsl:for-each select="@*">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:for-each>
		<!-- Find all Subsystems --> 
        <xsl:copy-of select="PromptDuringRestore"/>
        <!--xsl:apply-templates select="Information"/-->
		<!-- xsl:apply-templates select="DataModels"/ -->
		<xsl:apply-templates select="Subsystem[@Name='Communications']">
            <xsl:with-param name="SubsystemDefaults" select="$CommDefaults/Subsystem[@Name='Communications']"/>
        </xsl:apply-templates>
        <xsl:apply-templates select="Subsystem[@Name='Funk Security']">
            <xsl:with-param name="SubsystemDefaults" select="$FunkDefaults/Subsystem[@Name='Funk Security']"/>
        </xsl:apply-templates>
		<xsl:apply-templates select="Subsystem[@Name='IQueue']">
			<xsl:with-param name="SubsystemDefaults" select="$IQueueDefaults/Subsystem[@Name='IQueue']"/>
		</xsl:apply-templates>
    <xsl:apply-templates select="Subsystem[@Name='WWAN Radio']">
      <xsl:with-param name="SubsystemDefaults" select="$WWANDefaults/Subsystem[@Name='WWAN Radio']"/>
    </xsl:apply-templates>
	
	</DevInfo>
</xsl:template>
    
<xsl:template match="Information">
	<Information>
		<!-- Bring over all attributes one for one -->
		<xsl:for-each select="@*">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:for-each>
		<xsl:apply-templates select="OSName"/>
		<xsl:apply-templates select="IVAVersion"/>
        <xsl:apply-templates select="UniqueID"/>
	</Information>
</xsl:template>

<xsl:template match="UniqueID">
	<UniqueID>
		<!-- Bring over all attributes one for one -->
		<xsl:for-each select="@*">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:for-each>
		<!-- output the value of the element -->
		<xsl:value-of select="."/>
	</UniqueID>
</xsl:template>

<xsl:template match="OSName">
	<OSName>
		<!-- Bring over all attributes one for one -->
		<xsl:for-each select="@*">
			<xsl:attribute name="{name()}">
        <xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:for-each>
		<!-- output the value of the element -->
		<xsl:value-of select="."/>
	</OSName>
</xsl:template>

<xsl:template match="IVAVersion">
	<IVAVersion>
		<!-- Bring over all attributes one for one -->
		<xsl:for-each select="@*">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:for-each>
		<!-- output the value of the element -->
		<xsl:value-of select="."/>
	</IVAVersion>
</xsl:template>

<xsl:template match="DataModels">
	<DataModels>
		<!-- Bring over all attributes one for one -->
		<xsl:for-each select="@*">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:for-each>
		<xsl:apply-templates select="File"/>
		<xsl:apply-templates select="Pluggable"/>
	</DataModels>
</xsl:template>

<xsl:template match="File">
	<File>
		<!-- Bring over all attributes one for one -->
		<xsl:for-each select="@*">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:for-each>
	</File>
</xsl:template>

<xsl:template match="Pluggable">
	<Pluggable>
		<!-- Bring over all attributes one for one -->
		<xsl:for-each select="@*">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:for-each>
	</Pluggable>
</xsl:template>

  <!-- Handle WWAN settings -->
  <xsl:template match="Subsystem[@Name='WWAN Radio']">
    <xsl:param name="SubsystemDefaults"/>
    <xsl:variable name="SubsystemName" select="string(@Name )"/>
    <xsl:variable name="WWANSettings">
      <Subsystem>          
        <!-- Bring over all attributes one for one -->
        <xsl:for-each select="@*">
          <xsl:attribute name="{name()}">
            <xsl:value-of select="."/>
          </xsl:attribute>
        </xsl:for-each>
        <!-- process all groups and fields -->         
          <xsl:apply-templates select="Field">
            <xsl:with-param name="Path" select="concat('/',@Name)"/>
            <xsl:with-param name="FieldDefaults" select="$SubsystemDefaults[@Name=$SubsystemName]"/>
            <xsl:with-param name="RemapFields" select="$SubsystemDefaults//Field[@Remap]"/>
          </xsl:apply-templates>
          <xsl:apply-templates select="Group">
            <xsl:with-param name="Path" select="concat('/',@Name)"/>
            <xsl:with-param name="GroupDefaults" select="$SubsystemDefaults[@Name=$SubsystemName]"/>
            <xsl:with-param name="RemapFields" select="$SubsystemDefaults//Field[@Remap]"/>
          </xsl:apply-templates>
      </Subsystem>
    </xsl:variable>

    <!--Include WWAN Settings only of if contains any Field with a value in it -->
    <xsl:if test="count(msxsl:node-set($WWANSettings)//Field[.!='']) > 0">
      <xsl:copy-of select="$WWANSettings"/>
    </xsl:if>
    
  </xsl:template>
  
  
  <!-- Handle each subsystem -->
  <xsl:template match="Subsystem">
    <xsl:param name="SubsystemDefaults"/>
    <xsl:variable name="SubsystemName" select="string(@Name )"/>
	  <xsl:variable name="SubsystemSettings">
		  <Subsystem>
			  <!-- Bring over all attributes one for one -->
			  <xsl:for-each select="@*">
				  <xsl:attribute name="{name()}">
					  <xsl:value-of select="."/>
				  </xsl:attribute>
			  </xsl:for-each>
			  <!-- process all groups and fields -->
			  <xsl:apply-templates select="Field">
				  <xsl:with-param name="Path" select="concat('/',@Name)"/>
				  <xsl:with-param name="FieldDefaults" select="$SubsystemDefaults[@Name=$SubsystemName]"/>
				  <xsl:with-param name="RemapFields" select="$SubsystemDefaults//Field[@Remap]"/>
			  </xsl:apply-templates>
			  <xsl:apply-templates select="Group">
				  <xsl:with-param name="Path" select="concat('/',@Name)"/>
				  <xsl:with-param name="GroupDefaults" select="$SubsystemDefaults[@Name=$SubsystemName]"/>
				  <xsl:with-param name="RemapFields" select="$SubsystemDefaults//Field[@Remap]"/>
			  </xsl:apply-templates>
		  </Subsystem>

	  </xsl:variable>
	  <xsl:if test="count(msxsl:node-set($SubsystemSettings)//Field[.!='']) > 0">
		  <xsl:copy-of select="$SubsystemSettings"/>
	  </xsl:if>

  </xsl:template>

	<!--Include WWAN Settings only of if contains any Field with a value in it -->


	<!-- Handle the Funk  subsystem -->
<xsl:template match="Subsystem[@Name='Funk Security']">
    <xsl:param name="SubsystemDefaults"/>
    <xsl:variable name="ActiveProfile" select="./Field[@Name='ActiveProfile']"/>
    <!--xsl:comment>Active Profile = <xsl:value-of select="$ActiveProfile"/></xsl:comment-->
    <xsl:variable name="SubsystemName" select="string(@Name )"/>
    <!--xsl:comment>Zero Config1 = <xsl:value-of select="$ZeroConfig"/></xsl:comment-->
    <!--xsl:comment>Zero Config2 = <xsl:value-of select="$ZeroConfig2"/></xsl:comment-->
    <xsl:if test="$ZeroConfig = 'Off' or $ZeroConfig2 = 'Off'">
	<!--xsl:comment>Funk is on></xsl:comment-->
        <Subsystem>
            <!-- Bring over all attributes one for one -->
            <xsl:for-each select="@*">
                <xsl:attribute name="{name()}">
                    <xsl:value-of select="."/>
                </xsl:attribute>
            </xsl:for-each>
            <!-- process all groups and fields -->
	    <xsl:apply-templates select="Field">
                <xsl:with-param name="Path" select="concat('/',@Name)"/>
                <xsl:with-param name="FieldDefaults" select="$SubsystemDefaults[@Name=$SubsystemName]"/>
	        <xsl:with-param name="RemapFields" select="$SubsystemDefaults//Field[@Remap]"/>
	    </xsl:apply-templates>
            <xsl:apply-templates select="Group[@Instance=$ActiveProfile]">
                <xsl:with-param name="Path" select="concat('/',@Name)"/>
                <xsl:with-param name="GroupDefaults" select="$SubsystemDefaults[@Name=$SubsystemName]"/>
	        <xsl:with-param name="RemapFields" select="$SubsystemDefaults//Field[@Remap]"/>
            </xsl:apply-templates>	
        </Subsystem>
    </xsl:if>


</xsl:template>

<xsl:template match="Group">
    <xsl:param name="Path"/>
    <xsl:param name="GroupDefaults"/>
    <xsl:param name="RemapFields"/>
    <xsl:variable name="GroupName" select="string(@Name )"/>
    <xsl:variable name="BasePath" select="concat($Path,'/',$GroupName)"/>
    <!--xsl:variable name="Instance" select="@Instance"/-->
	<xsl:variable name="GroupSettings">
		<xsl:variable name="CurrPath">
       <xsl:choose>
          <xsl:when test="@Instance"><xsl:value-of select="concat($BasePath,':',@Instance)"/></xsl:when>
          <xsl:otherwise><xsl:value-of select="$BasePath"/></xsl:otherwise>  
       </xsl:choose>
    </xsl:variable>
    <!--xsl:comment>Group path = <xsl:value-of select="$CurrPath"/></xsl:comment-->
	<Group>
		<!-- Bring over all attributes one for one -->
		<xsl:for-each select="@*">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:for-each>
		<!-- process all groups and fields -->
		<xsl:apply-templates select="Field">
            <xsl:with-param name="Path" select="string($CurrPath)"/>
            <xsl:with-param name="FieldDefaults" select="$GroupDefaults/Group[@Name=$GroupName]"/>
	    <xsl:with-param name="RemapFields" select="$RemapFields"/>
        </xsl:apply-templates>	
		<xsl:apply-templates select="Group">
            <xsl:with-param name="Path" select="string($CurrPath)"/>
            <xsl:with-param name="GroupDefaults" select="$GroupDefaults/Group[@Name=$GroupName]"/>
	    <xsl:with-param name="RemapFields" select="$RemapFields"/>
        </xsl:apply-templates>	
	</Group>
	</xsl:variable>
	<xsl:if test="count(msxsl:node-set($GroupSettings)//Field[.!='']) > 0">
		<xsl:copy-of select="$GroupSettings"/>
	</xsl:if>

</xsl:template>

  <xsl:template match="Field">
    <xsl:param name="Path"/>
    <xsl:param name="FieldDefaults"/>
    <xsl:param name="RemapFields"/>
    <xsl:variable name="value" select="normalize-space(.)"/>
    <xsl:variable name="FieldName" select="string(@Name)"/>
    <xsl:variable name="CurrPath" select="concat($Path,'/',$FieldName)"/>
    <xsl:variable name="RemapPath" select="translate($CurrPath,'/','\')"/>
    <xsl:variable name="PromptFor" select="$PromptFields/Field[@Path=$RemapPath]"/>

    <!-- Copy the PasswordPrompt field over regardless of what it is set to
         This is needed because some devices default to Enabled and some to Disabled. >
      <Field Name="Field[@Name='PasswordPrompt']">
         <xsl:value-of select="."/>
      </Field-->
	<xsl:apply-templates select="Field[@Name='PasswordPrompt']"/>

    <!-- Filter out the Radio measurement field.  It's not needed to get a device connected-->
    <xsl:choose>
      <xsl:when test="normalize-space(@Name) = normalize-space('Radio Measurement')">
      <!-- Do Nothing
      <xsl:comment>****  Filtering RadioMeasurement out  ****</xsl:comment> -->
    </xsl:when>
    
    <!-- Filter out the PasswordPrompt field.  It is handled above. -->
      <xsl:when test="normalize-space(@Name) = normalize-space('PasswordPrompt')">
      <!-- Do Nothing
      <xsl:comment>****  Filtering PasswordPrompt out  ****</xsl:comment> -->
    </xsl:when>
    
    <!-- Copy the Radio Enabled field over regardless of what it is set to
         This is needed because some devices default to Radio On and Some to OFF.
         Still need to copy this over even if everything else is default because
         user may want to use the default INTERMEC SSID -->
      <xsl:when test="normalize-space(@Name) = normalize-space('Radio Enabled')">
        <Field>
          <xsl:for-each select="@*">
          <xsl:attribute name="{name()}">
            <xsl:value-of select="."/>
          </xsl:attribute>
        </xsl:for-each>
          <xsl:value-of select="$value"/>
        </Field>

      </xsl:when>
      <xsl:otherwise>

        <xsl:variable name="Default">
          <xsl:choose>
            <xsl:when test="$RemapFields[@Remap=$RemapPath]">
              <xsl:value-of select="string(normalize-space($RemapFields[@Remap=$RemapPath]/@Default))"/>
            </xsl:when>
            <!--normalize-space(@Name) = normalize-space('Radio Measurement')-->
            <xsl:when test="$FieldDefaults/Field[@Name=$FieldName]/@Default='-blank-'">
              <xsl:value-of select="string('')"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="string(normalize-space($FieldDefaults/Field[@Name=$FieldName]/@Default))"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:variable name="Editable">
          <xsl:choose>
            <xsl:when test="$RemapFields[@Remap=$RemapPath]">
              <xsl:value-of select="string(normalize-space($RemapFields[@Remap=$RemapPath]/@Editable))"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="string(normalize-space($FieldDefaults/Field[@Name=$FieldName]/@Editable))"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:variable name="Hidden">
          <xsl:choose>
            <xsl:when test="$RemapFields[@Remap=$RemapPath]">
              <xsl:value-of select="string(normalize-space($RemapFields[@Remap=$RemapPath]/@Hidden))"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="string(normalize-space($FieldDefaults/Field[@Name=$FieldName]/@Hidden))"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <xsl:if test="(($value != $Default) or $PromptFor) and (($Editable != 'false') and ($Hidden != 'true'))">
          <!--xsl:comment>Field path = <xsl:value-of select="$CurrPath"/></xsl:comment-->
          <!--xsl:comment>Field-<xsl:value-of select="$FieldName"/>,Default=<xsl:value-of select="$Default"/></xsl:comment-->
          <!--xsl:comment>Editable-<xsl:value-of select="$Editable"/></xsl:comment-->
          <!--xsl:comment>Count of remap nodes = <xsl:value-of select="count($RemapFields)"/></xsl:comment-->
          <Field>
            <!-- Bring over all attributes one for one -->
            <xsl:for-each select="@*">
              <xsl:attribute name="{name()}">
                <xsl:value-of select="."/>
              </xsl:attribute>
            </xsl:for-each>

            <xsl:if test="$PromptFor">
              <!-- Bring over Type and SubType from PromptFor -->
              <xsl:attribute name="Type">
                <xsl:value-of select="$PromptFor/@Type"/>
              </xsl:attribute>
              <xsl:attribute name="SubType">
                <xsl:value-of select="$PromptFor/@Subtype"/>
              </xsl:attribute>
              <!-- Add the Prompt Attribute -->
              <xsl:attribute name="Prompt">true</xsl:attribute>
            </xsl:if>

            <!-- output the value of the element -->
            <xsl:value-of select="$value"/>
          </Field>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>
    
  </xsl:template>
  
	<xsl:template match="Field[@Name='PasswordPrompt']">
		<xsl:copy-of select="."/>
	</xsl:template>

</xsl:stylesheet>
