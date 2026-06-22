'use strict';

const STORAGE_KEY = 'plm_api_key';

function getApiKey() {
  return localStorage.getItem(STORAGE_KEY) || '';
}

function saveApiKey(key) {
  localStorage.setItem(STORAGE_KEY, key);
}

function clearApiKey() {
  localStorage.removeItem(STORAGE_KEY);
}

function renderAuthSection() {
  const section = document.getElementById('auth-section');
  const key = getApiKey();

  if (key) {
    section.innerHTML =
      '<span class="api-key-set-label">API key set</span>' +
      '<button class="btn btn-secondary" id="btn-clear-key">Clear Key</button>';

    document.getElementById('btn-clear-key').addEventListener('click', () => {
      clearApiKey();
      renderAuthSection();
    });
  } else {
    section.innerHTML =
      '<input id="api-key-input" class="api-key-input" type="password"' +
      ' placeholder="Enter the API key…" autocomplete="off" spellcheck="false">' +
      '<button class="btn btn-primary" id="btn-save-key">Save</button>';

    const input = document.getElementById('api-key-input');
    const btn = document.getElementById('btn-save-key');

    btn.addEventListener('click', () => {
      const value = input.value.trim();
      if (!value) return;
      saveApiKey(value);
      renderAuthSection();
    });

    input.addEventListener('keydown', (e) => {
      if (e.key === 'Enter') btn.click();
    });

    input.focus();
  }
}

document.addEventListener('DOMContentLoaded', renderAuthSection);
