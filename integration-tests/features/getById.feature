Feature: Get glossary term by ID

  Background:
    * url apiHost

  Scenario: get a glossary term by its ID
    Given path 'Terms', 'Cancer.gov', 'Patient', 'en',
    When method get
    Then status 200
