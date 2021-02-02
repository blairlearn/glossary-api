Feature: GetById with useFallback=true with expected errors.

    Background:
        * url apiHost

    Scenario Outline: Given the dictionary, audience, language, id, and useFallback = true,
        validate the error thrown.

        Given path 'Terms', dictionary, audience, language, id
        And param useFallback = true
        When method get
        Then status 404
        And match response == read( expected )

        Examples:
            | dictionary | audience             | language | id       | expected                                                  |
            | Unknown    | Patient              | en       | 43966    | error-fallback-unknown-dictionary-patient-en-43966.json   |
            | Unknown    | HealthProfessional   | en       | 43966    | error-fallback-unknown-dictionary-hp-en-43966.json        |
