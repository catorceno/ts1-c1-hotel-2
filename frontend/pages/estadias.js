import { createSidebar } from '../widgets/sidebar.js';
import { createBookingPanel } from '../widgets/bookingsPanel.js';

export default function createEstadiasPage() {
  const page = document.createElement('div');
  page.className = 'app-shell';

  const sidebar = createSidebar();
  const bookingPanel = createBookingPanel();

  page.append(sidebar, bookingPanel);
  return page;
}
