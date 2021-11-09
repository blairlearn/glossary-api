Feature: Verify that GetByName returns the correct data structure.

    Background:
        * url apiHost

    Scenario Outline: Given the dictionary, audience, language, and pretty url name
        for dictionary: '<dictionary>', audience: '<audience>', language: '<language>', name: '<name>'

        Given path 'Terms', dictionary, audience, language, name
        When method get
        Then status 200
        And match response == read( expected )

        Examples:
            | dictionary | audience | language | name   | expected                              |
            | cancer.gov | patient  | en       | breast | data-structure-media-and-related.json |
            | cancer.gov | patient  | en       | a33    | data-structure-no-media.json          |
            | cancer.gov | patient  | es       | a33    | data-structure-no-pronunciation.json  |
