import MDP from 'react-multi-date-picker';





const mdp = MDP as unknown as { default?: unknown };
const DatePicker = (mdp.default ?? MDP) as typeof MDP;

export default DatePicker;
