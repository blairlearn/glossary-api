Feature: GetByName with useFallback=true with expected errors.

    Background:
        * url apiHost

    Scenario Outline: Given the dictionary, audience, language, prettyUrlName, and useFallback = true,
        validate the error thrown.

        Given path 'Terms', dictionary, audience, language, prettyUrlName
        And param useFallback = true
        When method get
        Then status 404
        And match response == read( expected )

        Examples:
            | dictionary | audience             | language | prettyUrlName         | expected                                                                |
            | Unknown    | Patient              | en       | clinical-resistance   | error-fallback-unknown-dictionary-patient-en-clinical-resistance.json   |
            | Unknown    | HealthProfessional   | en       | clinical-resistance   | error-fallback-unknown-dictionary-hp-en-clinical-resistance.json        |
