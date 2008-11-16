<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="@* | node()">
      <html>
        <body>
          <h2>My CD Collection</h2>
          <table border="1">
            <tr bgcolor="#9acd32">
              <th>Title</th>
              <th>Artist</th>
            </tr>
            <tr>
              <td>
                <xsl:value-of select="/SPWeb/Storage"/>
              </td>
              <td>
                <xsl:value-of select="/SPWeb/StorageMaximumLeveL"/>
              </td>
            </tr>
          </table>
        </body>
      </html>

    </xsl:template>
</xsl:stylesheet>
