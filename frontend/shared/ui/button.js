export function createButton({ text, type = 'button', variant = 'primary', onClick, disabled = false }) {
  const button = document.createElement('button');
  button.type = type;
  button.textContent = text;
  button.className = `button button--${variant}`;
  button.disabled = disabled;
  if (onClick) {
    button.addEventListener('click', onClick);
  }
  return button;
}
