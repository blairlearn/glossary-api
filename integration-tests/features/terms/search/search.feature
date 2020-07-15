Feature: Search with expected success.

    Background:
        * url apiHost

    Scenario Outline: Given the dictionary, audience, language, and search text,
        validate the search result.

        Given path 'Terms', 'search', dictionary, audience, language, search
        And param matchType = match
        When method get
        Then status 200
        And match response == read( expected )

        Examples:
            | dictionary | audience            | language | search                  | match     | expected                                        |
            # Path separator in term name
            | Cancer.gov | Patient             | en       | 2/3                     | contains  | search-contains-cgov-patient-en-two-thirds.json |
            | Cancer.gov | Patient             | es       | 2/3                     | contains  | search-contains-cgov-patient-es-two-thirds.json |
            # Begins/Contain searches with exact matches
            | Cancer.gov | Patient             | en       | MeV linear accelerator  | begins    | search-begins-exact-match-mev-linear.json       |
            | Cancer.gov | Patient             | en       | MeV linear accelerator  | contains  | search-contains-exact-match-mev-linear.json     |
            # Search for exact match when one does exist.
            | Cancer.gov | Patient             | en       | lung                    | exact     | search-exact-cgov-patient-en-one-word.json           |
            | Cancer.gov | Patient             | es       | kava kava               | exact     | search-exact-cgov-patient-es-with-space.json         |
            | Cancer.gov | Patient             | en       | CIN 2/3                 | exact     | search-exact-cgov-patient-en-with-slash.json         |
            | Cancer.gov | Patient             | es       | úvula                   | exact     | search-exact-cgov-patient-es-with-spanish-char.json  |
            | Cancer.gov | Patient             | en       | μL                      | exact     | search-exact-cgov-patient-en-greek-char.json         |
            | Cancer.gov | Patient             | en       | [18F]SPA-RQ             | exact     | search-exact-cgov-patient-en-square-bracket.json     |
            | Genetics   | HealthProfessional  | en       | z-score                 | exact     | search-exact-gen-patient-en-with-dash.json           |
            # Search for exact match when none should exist.
            | Cancer.gov | Patient             | en       | chicken                 | exact     | search-exact-mismatch-emptry-result.json        |
            | Cancer.gov | Patient             | es       | biopsia de pul          | exact     | search-exact-mismatch-emptry-result.json        |
