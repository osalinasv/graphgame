﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameInputController : MonoBehaviour {

	public static GameInputController Instance { get; private set; }

	private GameController gameController;

	public delegate void OnPositionChanged(float x, float y, float z);
	public event OnPositionChanged positionChangedEvent;

	public delegate void OnRotationChanged(float x, float y);
	public event OnRotationChanged rotationChangedEvent;

	public delegate void OnScaleChanged(float x, float y, float z);
	public event OnScaleChanged scalenChangedEvent;

	public delegate void OnResetGame();
	public event OnResetGame resetGameEvent;

	[SerializeField]
	private Button solveButton;

	[Header("Try Toggles")]
	[SerializeField]
	private Transform tryTogglesParent;
	[SerializeField]
	private GameObject tryTogglePreset;

	[Header("Labels")]
	[SerializeField]
	private Text transformAttemptsLabel;

	[Header("Position Fields")]
	[SerializeField]
	private Button positionButton;
	[SerializeField]
	private InputField[] positionInputs = new InputField[3];

	[Header("Rotation Fields")]
	[SerializeField]
	private Button rotateButton;
	[SerializeField]
	private InputField[] rotationInputs = new InputField[2];

	[Header("Scale Fields")]
	[SerializeField]
	private Button scaleButton;
	[SerializeField]
	private InputField[] scaleInputs = new InputField[3];

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;

		// Delegates and event suscriptions

		this.gameController = GameController.Instance;

		if (gameController) {
			gameController.tryAttemptsChangedEvent += setTryAttemptsToggles;
			gameController.transformAttemptsChangedEvent += setTransformAttempsLabel;
		}
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.T)) {
			setPosition();
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			setRotation();
		}

		if (Input.GetKeyDown(KeyCode.S)) {
			setScale();
		}
	}

	public void setTryAttemptsToggles(int amount) {
		List<Toggle> toggles = new List<Toggle>();
		GameObject toggleObject = null;
		Toggle toggleComponent = null;

		if (tryTogglesParent.childCount < amount) {
			foreach (Transform child in tryTogglesParent) {
				DestroyImmediate(child.gameObject);
			}

			for (int i = 0; i < amount; i++) {
				toggleObject = Instantiate(tryTogglePreset, tryTogglesParent);
				toggleComponent = toggleObject.GetComponent<Toggle>();

				toggles.Add(toggleComponent);
			}
		} else {
			foreach (Transform child in tryTogglesParent) {
				toggleComponent = child.GetComponent<Toggle>();
				toggles.Add(toggleComponent);
			}
		}

		foreach (Toggle toggle in toggles) {
			if (amount-- > 0) {
				toggle.isOn = true;
			} else {
				toggle.isOn = false;
			}
		}
	}

	public void setTransformAttempsLabel(int amount) {
		if (transformAttemptsLabel) {
			transformAttemptsLabel.text = (amount >= 0 ? amount : 0).ToString();
		}
	}

	public void runSolveTry() {
		if (gameController && gameController.solveTryAttempts > 0) {
			if (gameController.startPlaneSequence()) {
				toggleSolveButton(false);
				toggleTransformButtons(false);
			}
		}
	}

	public void toggleSolveButton(bool isActive) {
		if (solveButton) {
			solveButton.interactable = isActive;
		}
	}

	public void toggleTransformButtons(bool isActive) {
		if (positionButton) {
			positionButton.interactable = isActive;
		}

		if (rotateButton) {
			rotateButton.interactable = isActive;
		}

		if (scaleButton) {
			scaleButton.interactable = isActive;
		}
	}

	private float readInputValue(InputField input, float defaultValue = 0.0f) {
		string inputText = input.text;
		input.text = null;

		return (!string.IsNullOrEmpty(inputText)) ? float.Parse(inputText) : defaultValue;
	}

	public void setPosition() {
		float[] inputValues = positionInputs.Select(input => readInputValue(input)).ToArray();

		if (gameController.substractAndTestTransformAttempts()) {
			if (positionChangedEvent != null && inputValues.Length == positionInputs.Length) {
				positionChangedEvent(inputValues[0], inputValues[1], inputValues[2]);
			}
		}
	}

	public void setRotation() {
		float[] inputValues = rotationInputs.Select(input => readInputValue(input)).ToArray();

		if (gameController.substractAndTestTransformAttempts()) {
			if (rotationChangedEvent != null && inputValues.Length == rotationInputs.Length) {
				rotationChangedEvent(inputValues[0], inputValues[1]);
			}
		}
	}

	public void setScale() {
		float[] inputValues = scaleInputs.Select(input => readInputValue(input, 1.0f)).ToArray();

		if (gameController.substractAndTestTransformAttempts()) {
			if (scalenChangedEvent != null && inputValues.Length == scaleInputs.Length) {
				scalenChangedEvent(inputValues[0], inputValues[1], inputValues[2]);
			}
		}
	}

	public void resetAll() {
		if (resetGameEvent != null) {
			resetGameEvent();
		}
	}
}