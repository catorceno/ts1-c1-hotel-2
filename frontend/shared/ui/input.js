export function createInputField({ label, name, type = 'text', placeholder = '', value = '', required = false, onInput }) {
  const field = document.createElement('div');
  field.className = 'field';

  const labelElement = document.createElement('label');
  labelElement.htmlFor = name;
  labelElement.textContent = label;

  const input = document.createElement('input');
  input.id = name;
  input.name = name;
  input.type = type;
  input.placeholder = placeholder;
  input.value = value;
  input.required = required;
  input.className = 'input';
  if (onInput) {
    input.addEventListener('input', onInput);
  }

  field.append(labelElement, input);
  return { field, input };
}
