Feature: Verify that GetById returns the correct data structure.

    Background:
        * url apiHost

    Scenario Outline: Given the dictionary, audience, language, and id
        for dictionary: '<dictionary>', audience: '<audience>', language: '<language>', id: '<id>'

        Given path 'Terms', dictionary, audience, language, id
        When method get
        Then status 200
        And match response == read( expected )

        Examples:
            | dictionary | audience | language  | id    | expected                                |
            | cancer.gov | patient  | en        | 304766| data-structure-media-and-related.json   |
            | cancer.gov | patient  | en        | 46722 | data-structure-no-media.json            |
            | cancer.gov | patient  | es        | 46722 | data-structure-no-pronunciation.json    |
