using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NumberTextConverter {
    public static class Converter {
        public static string ConvertAmountToText(string amount) {
            var moneySignatureIndex = amount.IndexOfAny("$€₺".ToCharArray());
            var moneySignature = string.Empty;
            if (moneySignatureIndex != -1) {
                moneySignature = amount[moneySignatureIndex].ToString();
                amount = amount.Replace(moneySignature, string.Empty).Trim();
            }
            double.TryParse(amount, out double moneyValue);
            if (moneyValue < 0) {
                return string.Empty;
            }

            var moneySignatureText = GetMoneySignatureText(moneySignature);
            var moneyValueText = GetMoneyValueText(moneyValue);

            var sb = new StringBuilder();
            sb.Append(moneyValueText.Item1).Append(" ").Append(moneySignatureText.Item1);
            if (!string.IsNullOrEmpty(moneyValueText.Item2)) {
                sb.Append(moneyValueText.Item2).Append(" ").Append(moneySignatureText.Item2);
            }
            return sb.ToString();
        }

        private static (string, string) GetMoneyValueText(double moneyValue) {
            try {
                var firstPart = ConvertNumbersToText(Convert.ToUInt32(Math.Truncate(moneyValue)));
                var decimalPart = Convert.ToUInt32((moneyValue - Math.Truncate(moneyValue)) * 100);
                if (decimalPart < 10) {
                    decimalPart = decimalPart * 10;
                }
                var secondPart = ConvertNumbersToText(decimalPart);
                return (firstPart, secondPart);
            } catch (Exception) {
                return (string.Empty, string.Empty);
            }
        }

        private static string ConvertNumbersToText(uint number) {
            var numbers = number.ToString().Select(t => uint.Parse(t.ToString())).ToArray();
            if (numbers.Length == 1) {
                return ConvertDigitToString(number);
            }
            if (numbers.Length == 2) {
                return numbers[0] == 1 ? ConvertTeensToString(number) : ConvertHighTensToString(numbers[0], numbers[1]);
            }

            if (numbers.Length == 3) {
                return ConvertHundredsToText(numbers[0], number);
            }

            if (numbers.Length > 3) {
                var newNumbers = new List<uint>();
                var reversedNumbers = numbers.Reverse().ToArray();
                for (var i = 0; i < reversedNumbers.Length; i = i + 3) {
                    var size = reversedNumbers.Length - 1 - i > 2 ? 3 : reversedNumbers.Length - 1 - i + 1;
                    newNumbers.Add(GetNewNumber(reversedNumbers, i, size));
                }
                return ConvertBigNumbersToText(newNumbers);
            }
            return string.Empty;
        }




        private static string ConvertDigitToString(uint i) {
            switch (i) {
                case 0: return "";
                case 1: return " one";
                case 2: return " two";
                case 3: return " three";
                case 4: return " four";
                case 5: return " five";
                case 6: return " six";
                case 7: return " seven";
                case 8: return " eight";
                case 9: return " nine";
                default:
                    throw new IndexOutOfRangeException($"{i} not a digit");
            }
        }

        private static string ConvertTeensToString(uint i) {
            switch (i) {
                case 10: return " ten";
                case 11: return " eleven";
                case 12: return " twelve";
                case 13: return " thirteen";
                case 14: return " fourteen";
                case 15: return " fiveteen";
                case 16: return " sixteen";
                case 17: return " seventeen";
                case 18: return " eighteen";
                case 19: return " nineteen";
                default:
                    throw new IndexOutOfRangeException($"{i} not a teen");
            }
        }

        private static string ConvertHighTensToString(uint tenPart, uint digitPart) {
            string tensStr;
            switch (tenPart) {
                case 2: tensStr = " twenty"; break;
                case 3: tensStr = " thirty"; break;
                case 4: tensStr = " forty"; break;
                case 5: tensStr = " fifty"; break;
                case 6: tensStr = " sixty"; break;
                case 7: tensStr = " seventy"; break;
                case 8: tensStr = " eighty"; break;
                case 9: tensStr = " ninety"; break;
                default:
                    throw new IndexOutOfRangeException($"{tenPart} not in range 20-99");
            }
            if (digitPart == 0) return tensStr;
            var onesStr = ConvertDigitToString(digitPart);
            return tensStr + " " + onesStr;
        }

        private static string ConvertHundredsToText(uint u, uint number) {
            var hundredPart = ConvertDigitToString(u) + " hundred";
            var leftNumber = number - u * 100;
            return hundredPart + ConvertNumbersToText(leftNumber);
        }

        private static string ConvertBigNumbersToText(List<uint> number) {
            var sb = new StringBuilder();
            for (int i = number.Count - 1; i >= 0; i--) {
                sb.Append(ConvertNumbersToText(number[i]));
                if (i == 1) {
                    sb.Append(" thousand ");
                } else if (i == 2) {
                    sb.Append(" million ");
                } else if (i == 3) {
                    sb.Append(" billion ");
                }
            }
            return sb.ToString();
        }

        public static uint GetNewNumber(uint[] data, int index, int length) {
            var result = new uint[length];
            Array.Copy(data, index, result, 0, length);
            result = result.Reverse().ToArray();
            uint newNumber = 0;
            for (var i = 0; i < length; i++) {
                newNumber += (uint)(result[i] * Convert.ToInt32(Math.Pow(10, length - i - 1)));
            }
            return newNumber;
        }

        private static (string, string) GetMoneySignatureText(string moneySignature) {
            try {
                switch (moneySignature) {
                    case "$":
                        return (" dollars ", " cents ");
                    case "€":
                        return (" euro ", " cents ");
                    case "₺":
                        return (" liras ", " kuruses ");
                    default:
                        return (string.Empty, string.Empty);
                }
            } catch (Exception) {
                return (string.Empty, string.Empty);
            }
        }
    }
}
