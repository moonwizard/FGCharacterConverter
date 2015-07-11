<?xml version="1.0" encoding="utf-8"?>
<!-- Copyright (c) 2012, SmiteWorks USA LLC -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl xs" xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xsl:output method="xml" omit-xml-declaration="no" indent="yes"/>
  <xsl:template name="parse-dice-get-dice">
    <xsl:param name="diceField"/>
    <xsl:choose>
      <xsl:when test="contains($diceField, '+')">
        <xsl:value-of select="substring-before($diceField, '+')"/>
      </xsl:when>
      <xsl:when test="contains($diceField, '-')">
        <xsl:value-of select="substring-before($diceField, '-')"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$diceField"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="parse-dice-get-bonus">
    <xsl:param name="diceField"/>
    <xsl:choose>
      <xsl:when test="contains($diceField, '-')">
        <xsl:value-of select="concat('-', substring-after($diceField, '-'))"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="substring-after($diceField, '+')"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="outputTrait">
    <xsl:param name="trait"/>
    <xsl:param name="search"/>
    <xsl:element name="{$trait}">
      <xsl:attribute name="type">dice</xsl:attribute>
      <xsl:variable name="value">
        <xsl:value-of select="@value"/>
      </xsl:variable>
      <xsl:call-template name="parse-dice-get-dice">
        <xsl:with-param name="diceField" select="$value"/>
      </xsl:call-template>
    </xsl:element>

    <xsl:element name="{concat($trait,'Mod')}">
      <xsl:attribute name="type">number</xsl:attribute>
      <xsl:value-of select="'0'"/>
    </xsl:element>
  </xsl:template>
  <xsl:template name="outputTraitBonus">
    <xsl:param name="trait"/>
    <xsl:param name="search"/>
    <xsl:element name="{$trait}">
      <id-00001>
        <bonus type="number">
          <xsl:variable name="value">
            <xsl:value-of select="@value"/>
          </xsl:variable>
          <xsl:call-template name="parse-dice-get-bonus">
            <xsl:with-param name="diceField" select="$value"/>
          </xsl:call-template>
        </bonus>
      </id-00001>
    </xsl:element>
  </xsl:template>

  <xsl:output method="xml" indent="yes"/>
  <xsl:template match="/child::*">
    <root version="3.0" ccversion="3.0.5.A">
      <character>
        <xsl:for-each select="descendant::attributes/attribute">
          <xsl:variable name="trait">
            <xsl:value-of select="@name"/>
          </xsl:variable>
          <xsl:call-template name="outputTrait">
            <xsl:with-param name="trait" select="translate($trait,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')"/>
            <xsl:with-param name="search" select="$trait"/>
          </xsl:call-template>
        </xsl:for-each>
        <allies/>
        <armorlist>|CC.HLSW.ARMORLIST|</armorlist>
        <atplist>|CC.HLSW.POWERLIST|</atplist>
        <bonuslist>
          <xsl:for-each select="descendant::attributes/attribute">
            <xsl:variable name="trait">
              <xsl:value-of select="@name"/>
            </xsl:variable>
            <xsl:call-template name="outputTraitBonus">
              <xsl:with-param name="trait" select="translate($trait,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')"/>
              <xsl:with-param name="search" select="$trait"/>
            </xsl:call-template>
          </xsl:for-each>
        </bonuslist>
        <currency type="string">
          <xsl:value-of select="cash/@total"/>
        </currency>
        <cwlimit type="number">
          <xsl:value-of select="resources/resource[@name='Encumbrance']/@max"/>
        </cwlimit>
        <edges>|CC.HLSW.EDGELIST|</edges>
        <hindrances>|CC.HLSW.HINDRANCELIST|</hindrances>
        <inc type="number">0</inc>
        <invlist>|CC.HLSW.INVLIST|</invlist>
        <languages>
        </languages>
        <main>
          <bennies type="number">
            <xsl:value-of select="trackers/tracker[@name='Bennies']/@left"/>
          </bennies>
          <fatigue type="number">0</fatigue>
          <powerpoints type="number">
            <xsl:value-of select="trackers/tracker[@name='Power Points']/@left"/>
          </powerpoints>
          <powerpointsmax type="number">
            <xsl:value-of select="trackers/tracker[@name='Power Points']/@max"/>
          </powerpointsmax>
          <wounds type="number">0</wounds>
          <xp type="number">
            <xsl:value-of select="xp/@total"/>
          </xp>
        </main>
        <name type="string">
          <xsl:value-of select="/character/@name"/>
        </name>
        <notes type="string">
          <xsl:value-of select="personal/description/text()"/>
        </notes>
        <xsl:for-each select="descendant::traits/trait">
          <xsl:variable name="trait">
            <xsl:value-of select="@name"/>
          </xsl:variable>
          <xsl:element name="{translate($trait,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')}">
            <xsl:attribute name="type">number</xsl:attribute>
            <xsl:value-of select="@value"/>
          </xsl:element>
        </xsl:for-each>
        <race type="string">
          <xsl:value-of select="race/@name"/>
        </race>
        <rank type="string">
          <xsl:value-of select="rank/@name"/>
        </rank>
        <setting type="string">
          <xsl:value-of select="settings/@summary"/>
        </setting>
        <skills>|CC.HLSW.SKILLLIST|</skills>
        <weaponlist>|CC.HLSW.WEAPONLIST|</weaponlist>
        <wildcard type="number">1</wildcard>
        <arcaneType type="string">|CC.HLSW.ARCANETYPE|</arcaneType>
      </character>
    </root>
  </xsl:template>
</xsl:stylesheet>