# Input Method Analysis for Fallout Terminal Hacker

This document analyzes different approaches for entering potential passwords into the Fallout Terminal Hacker mobile application.

## Table of Contents

- [Context](#context)
- [Input Methods Analysis](#input-methods-analysis)
  - [Method 1: Space-Separated Text Input](#method-1-space-separated-text-input)
  - [Method 2: Combo Box with Word List](#method-2-combo-box-with-word-list)
  - [Method 3: Camera with OCR](#method-3-camera-with-ocr)
- [Alternative Methods](#alternative-methods)
  - [Method 4: Hybrid OCR with Manual Correction](#method-4-hybrid-ocr-with-manual-correction)
  - [Method 5: Speech-to-Text Input](#method-5-speech-to-text-input)
  - [Method 6: Clipboard-Based Input](#method-6-clipboard-based-input)
- [Comparison Summary](#comparison-summary)
- [Recommended Solution](#recommended-solution)
- [Implementation Plan](#implementation-plan)
- [Technical Considerations](#technical-considerations)

---

## Context

The Fallout Terminal Hacker mini-game presents players with 10-16 English words of equal length (4-15 characters), displayed on a retro-style green phosphor terminal screen. Words appear interspersed with random ASCII characters.

![Terminal Screen]

The application needs to capture these words efficiently to provide password suggestions. The input method significantly impacts user experience, as manual entry of potentially 200+ characters on a mobile device can be tedious.

---

## Input Methods Analysis

### Method 1: Space-Separated Text Input

**Description:** User types all words into a single text field, separated by spaces.

**Advantages:**
- Simplest implementation
- Minimal UI complexity
- No external dependencies
- Works offline
- Platform-agnostic

**Disadvantages:**
- High typo probability on mobile keyboards
- No validation during input
- Tedious for 10-16 words (potentially 200+ characters)
- Mobile keyboard slows input significantly
- No autocomplete support

**User Experience Rating:** ⭐⭐ (2/5)

**Implementation Complexity:** Low

**Recoommended Platforms**: Console (RAVEN) and Web (GHOST)

---

### Method 2: Combo Box with Word List

**Description:** User enters words one at a time using a combo box with autocomplete functionality. The combo box suggests words from a pre-loaded English word list. After the first word is entered, the list is filtered to words of matching length.

**Advantages:**
- Validated input (only real words accepted)
- Autocomplete accelerates input
- Automatic filtering by word length after first entry
- Reduced typos
- Immediate feedback on invalid entries

**Disadvantages:**
- Requires interaction for each word (10-16 separate entries)
- Needs a comprehensive English word list (100k+ words)
- Some game words may not be in standard dictionaries (e.g., "GURPS")
- Complex UI logic for filtering
- Initial word list loading time

**User Experience Rating:** ⭐⭐⭐½ (3.5/5)

**Implementation Complexity:** Medium

**Recoommended Platforms**: Web (GHOST) and MAUI (ECHELON)

---

### Method 3: Camera with OCR

**Description:** User photographs the terminal screen, and OCR technology extracts the words automatically.

**Advantages:**
- Fastest input method (single photo captures all words)
- Minimal user interaction required
- "Magical" user experience
- Reduces human error in transcription

**Disadvantages:**
- OCR accuracy challenges with stylized green phosphor display
- CRT screen curvature and scan lines affect recognition
- Lighting conditions impact results
- Need to filter words from random ASCII characters
- Larger app size due to OCR libraries
- Requires camera permissions
- May not work well with screen photos (moiré patterns)

**User Experience Rating:** ⭐⭐⭐⭐ (4/5) - when working correctly

**Implementation Complexity:** High

**Recoommended Platforms**: MAUI (ECHELON)
---

## Alternative Methods

### Method 4: Hybrid OCR with Manual Correction

**Description:** Camera captures the image and OCR extracts words, then results are displayed in an editable list where users can correct, add, or remove words.

**Advantages:**
- Combines OCR speed with manual validation
- Handles OCR errors gracefully
- Best of both worlds approach
- User maintains control over final word list

**Disadvantages:**
- Still requires OCR implementation
- Additional UI for editing
- Two-step process

**User Experience Rating:** ⭐⭐⭐⭐½ (4.5/5)

**Implementation Complexity:** High

**Recoommended Platforms**: MAUI (ECHELON)
---

### Method 5: Speech-to-Text Input

**Description:** User reads the words aloud, and speech recognition converts them to text.

**Advantages:**
- Hands-free input
- .NET MAUI supports speech recognition
- Could be faster than typing

**Disadvantages:**
- Doesn't work in noisy environments
- Privacy concerns (speaking passwords aloud)
- Recognition errors with unusual words
- Slower than camera-based input
- Requires microphone permissions

**User Experience Rating:** ⭐⭐ (2/5)

**Implementation Complexity:** Medium

**Recoommended Platforms**: MAUI (ECHELON)
---

### Method 6: Clipboard-Based Input

**Description:** For PC players, copy terminal text (if possible) and share with phone via clipboard sync or messaging.

**Advantages:**
- Perfect accuracy if text is copyable
- No manual typing needed

**Disadvantages:**
- Only works for PC players
- Requires clipboard sharing setup
- Terminal text usually not copyable in-game
- Complex multi-device workflow

**User Experience Rating:** ⭐½ (1.5/5)

**Implementation Complexity:** Low (but limited applicability)

**Recoommended Platforms**: Web (GHOST) and MAUI (ECHELON)
---

## Comparison Summary

| Method | Speed | Reliability | Complexity | Offline | Rating |
|--------|-------|-------------|------------|---------|--------|
| Text Input | ⭐⭐ | ⭐⭐ | ⭐ | ✅ | 2/5 |
| Combo Box | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ✅ | 3.5/5 |
| Camera OCR | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ | ⚠️ | 4/5 |
| Hybrid OCR | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⚠️ | 4.5/5 |
| Speech | ⭐⭐ | ⭐⭐ | ⭐⭐⭐ | ❌ | 2/5 |
| Clipboard | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐ | ✅ | 1.5/5 |

---

## Recommended Solution

We recommend implementing a **multi-tier input system** that provides multiple entry methods, allowing users to choose based on their preferences and circumstances.

### UI Mockup

![UI-Mockup]


### Priority Order

1. **Primary:** Combo Box with Autocomplete (reliable, always works)
2. **Secondary:** Camera OCR with Manual Correction (fastest when available)
3. **Tertiary:** Plain Text Input (fallback option)

---

## Implementation Plan

### Technology Stack

- **Framework:** .NET MAUI (successor to Xamarin, modern and supported)
- **Platform:** Android (initial target)
- **OCR Library:** Plugin.Maui.OCR or Tesseract
- **Testing:** xUnit with Shouldly assertions

### Project Structure

```
EnclaveEchelon/
├── src/Enclave.Echelon/
│   ├── Core/                           # Enclave.Core namespace
│   │   ├── Models/
│   │   │   ├── Password.cs
│   │   │   └── GameSession.cs
│   │   └── Services/
│   │       ├── IPasswordSolver.cs
│   │       ├── PasswordSolver.cs
│   │       ├── IOcrService.cs
│   │       ├── OcrService.cs
│   │       ├── IWordListService.cs
│   │       └── WordListService.cs
│   ├── Shared/                         # Enclave.Shared namespace
│   │   └── ViewModels/
│   │       ├── MainViewModel.cs
│   │       └── ResultViewModel.cs
│   ├── App/
│   │   ├── Maui/                       # ECHELON 2.1.7
│   │   │   ├── Views/
│   │   │   │   ├── MainPage.xaml
│   │   │   │   ├── CameraPage.xaml
│   │   │   │   └── ResultPage.xaml
│   │   ├── Web/                       # GHOST 1.2.4
│   │   └── Console/                    # RAVEN 0.4.0
│   └── Tests/                         # Enclave.Tests namespace
│       └── PasswordSolverTests.cs
```

### Development Phases

#### Phase 1: Core Algorithm
- Implement password matching logic
- Implement "best guess" selection algorithm
- Unit tests for solver logic

#### Phase 2: Basic UI with Combo Box
- Main page with word entry
- Combo box with autocomplete
- Word list display with edit/delete
- Results page with suggestions

#### Phase 3: OCR Integration
- Camera capture implementation
- Image preprocessing for green terminal screen
- OCR text extraction
- Word filtering and validation

#### Phase 4: Polish and Optimization
- UI/UX improvements
- Performance optimization
- Error handling
- User preferences

---

## Technical Considerations

### OCR Preprocessing for Fallout Terminal

The green phosphor terminal screen requires specific image preprocessing:

1. **Color Channel Extraction:** Isolate the green channel
2. **Contrast Enhancement:** Increase contrast to separate text from background
3. **Thresholding:** Convert to binary image
4. **Noise Reduction:** Remove scan lines and artifacts

### Word Filtering Logic

After OCR extraction, apply these filters:

1. **Pattern Match:** Only strings matching `[A-Z]{4,15}`
2. **Length Consistency:** All words must have same length
3. **Duplicate Removal:** Remove repeated words
4. **Dictionary Validation:** Optionally validate against word list

### Word List Optimization

For efficient autocomplete with 100k+ words:

1. **Trie Data Structure:** O(m) lookup where m = word length
2. **Lazy Loading:** Load word list on first use
3. **Length-Based Partitioning:** Separate lists by word length
4. **Compressed Storage:** Use efficient text compression

---

## Conclusion

The recommended approach combines the reliability of combo box input with the speed of OCR, giving users flexibility in how they enter passwords. Starting with the combo box implementation provides a solid foundation, while OCR can be added as a premium feature enhancement.

The hybrid approach ensures the application remains useful even when OCR fails, maintaining a good user experience across all scenarios.

[//]: #References-and-image-links
[Terminal Screen]: ../Images/Terminal.png
[UI-Mockup]: ../Images/UI-elements/UI-mockup.drawio.svg
