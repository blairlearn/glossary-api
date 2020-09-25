Feature: Expand letters of the alphabet

    Background:
        * url apiHost

    Scenario Outline: Given the dictionary, audience, language, and a letter of the alphabet,
        validate the search result.

        Given path 'Terms', 'search', dictionary, audience, language, letter
        And params {from: <from>, size: <size>}
        When method get
        Then status 200
        And match response == read( expected )

        Examples:
            | dictionary | audience            | language | letter | from | size | expected                      |
            # Check diacritics are handled correctly -- the expected data is tricky to update because additional
            # terms may change where a given term appears. So check that diacritics are included and vary the
            # from/size if needed.
            | Cancer.gov | Patient             | es       | u      | 0    | 100  | expand-with-diacritics-u.json |
            | Cancer.gov | Patient             | es       | a      | 0    | 100  | expand-with-diacritics-a.json |
            # English terms won't generally have diacritics.
            | Cancer.gov | Patient             | en       | a      | 0    | 20   | expand-english-a.json           |
            | Cancer.gov | Patient             | es       | g      | 0    | 20   | expand-spanish-g.json           |

